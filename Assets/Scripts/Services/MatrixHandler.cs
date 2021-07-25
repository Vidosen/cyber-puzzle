using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Matrix;
using Prototype.Scripts.Data;
using Prototype.Scripts.Matrix;
using Prototype.Scripts.Providers.Mono;
using Signals;
using UniRx;
using UnityEngine;

namespace Services
{
    public class MatrixHandler : MonoBehaviour
    {
        [SerializeField] private MatrixProvider _matrixProvider;
        [SerializeField] private CombinationsHandler _combinationsHandler;
        private GameMatrix _gameMatrix;
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        public readonly Subject<Unit> MatrixChanged = new Subject<Unit>();
        
        
#if DEBUG
        private void DebugMatrix()
        {
            Debug.Log("NEW MATRIX:\b");
            for (var i = 0; i < _gameMatrix.RowsSize; i++)
            {
                var resultStr = "| ";
                for (var j = 0; j < _gameMatrix.ColumnsSize; j++)
                {
                    var pos = (j, i);
                    resultStr += $"({pos.Item1},{pos.Item2})" + _gameMatrix[j, i].Value;
                    if (j == _gameMatrix.ColumnsSize - 1)
                    {
                        resultStr += " |\b\b";
                        continue;
                    }

                    resultStr += " | ";
                }

                Debug.Log(resultStr);
            }
        }
#endif
        public void InitMatrix(LevelSettings level)
        {
            _gameMatrix = _matrixProvider.CreateNew();
            _gameMatrix.ThisTransform.localPosition = Vector2.zero;
            if (!_gameMatrix.IsInitialized)
                _gameMatrix.InitializeFromLevelSO(level);

            MessageBroker.Default.Receive<MatrixSignals.VectorSwapRequest>()
                .Subscribe(request => SwapVectors(request.ActiveVector, request.PassiveVector))
                .AddTo(_compositeDisposable);
            OnMatrixChanged();
        }

        private void OnMatrixChanged()
        {
            DimAllMatrixCells(HighlightType.CombinationSequence);
            MatrixChanged.OnNext(Unit.Default);
#if DEBUG
            DebugMatrix();
#endif
        }

        public void DisposeMatrix()
        {
            if (_gameMatrix != null)
                _gameMatrix.Dispose();
            _compositeDisposable.Clear();
            _gameMatrix = null;
        }

        #region Highlight Methods

        public void DimAllMatrixCells(HighlightType type)
        {
            foreach (var matrixCell in _gameMatrix.AllCells)
                matrixCell.DimCell(type);
        }

        public void HighlightAllMatchingMatrixCells(int compValue, Color highlightColor, HighlightType type)
        {
            foreach (var matrixCell in _gameMatrix.AllCells.Where(cell => cell.Value == compValue))
                matrixCell.HighlightCell(highlightColor, type);
        }

        #endregion

        public void SwapVectors(BaseVector oneVector, BaseVector twoVector)
        {
            _gameMatrix.SwapVectors(oneVector, twoVector);
            OnMatrixChanged();
        }

        public List<MatrixCell> FindBestMatrixCombination(List<ICell> combination)
        {
            var foundCombinations = new List<List<MatrixCell>>();
            var firstCell = combination.FirstOrDefault();
            if (firstCell == null)
                throw new Exception("'combination' parameter doesn't contain any CombinationCell!");
            var startCells = _gameMatrix.AllCells.Where(mCell => mCell.Value == firstCell.Value);
            foreach (var cell in startCells)
            {
                var result = new List<MatrixCell>();
                FindMatrixCombination(cell, combination, 1, ref result);
                foundCombinations.Add(result);
            }

            return foundCombinations
                .OrderByDescending(c => c.Count)
                .FirstOrDefault();
        }

        public void ReplaceCell(MatrixCell cell)
        {
            int combinationValuesDifference;
            foreach (var key in _combinationsHandler.CombinationValuesMap.Keys)
            {
                if (_gameMatrix.MatrixValuesMap.ContainsKey(key) &&
                    (combinationValuesDifference = _combinationsHandler.CombinationValuesMap[key] - _gameMatrix.MatrixValuesMap[key]) > 0)
                {
                    _gameMatrix.ChangeCell(cell, key);
                    //Debug.Log($"Difference of VALUE {key} (combination - matrix) is {combinationValuesDifference}");
                    return;
                }
            }
            _gameMatrix.ChangeCell(cell);
        }
        
        
        private void FindMatrixCombination(MatrixCell cell, List<ICell> combination, int currentIndex,
            ref List<MatrixCell> resultCells)
        {
            if (resultCells == null) resultCells = new List<MatrixCell>();
            resultCells.Add(cell);
            if (currentIndex >= combination.Count)
                return;

            var pos = _gameMatrix.IndexOf(cell);
            var x = pos.x;
            var y = pos.y;

            if (x + 1 < _gameMatrix.ColumnsSize && _gameMatrix[x + 1, y].Value == combination[currentIndex].Value)
            {
                FindMatrixCombination(_gameMatrix[x + 1, y], combination, ++currentIndex, ref resultCells);
                return;
            }

            if (x - 1 >= 0 && _gameMatrix[x - 1, y].Value == combination[currentIndex].Value)
            {
                FindMatrixCombination(_gameMatrix[x - 1, y], combination, ++currentIndex, ref resultCells);
                return;
            }

            if (y + 1 < _gameMatrix.RowsSize && _gameMatrix[x, y + 1].Value == combination[currentIndex].Value)
            {
                FindMatrixCombination(_gameMatrix[x, y + 1], combination, ++currentIndex, ref resultCells);
                return;
            }

            if (y - 1 >= 0 && _gameMatrix[x, y - 1].Value == combination[currentIndex].Value)
            {
                FindMatrixCombination(_gameMatrix[x, y - 1], combination, ++currentIndex, ref resultCells);
            }
        }

        public int[] GenerateRandomCombination(int numOfCells)
        {
            var uniqueCombinations = new List<MatrixCell>();
            for (int i = 0; i < numOfCells; i++)
            {
                var randomCellIndex = UnityEngine.Random.Range(0, _gameMatrix.AllCells.Length);
                var newCell = _gameMatrix.AllCells[randomCellIndex];
                if (uniqueCombinations.Contains(newCell))
                {
                    i--;
                }
                else
                {
                    uniqueCombinations.Add(newCell);
                }
            }
            return uniqueCombinations.Select(cell => cell.Value).ToArray();
        }
        
        public int[] GenerateJointCombination(int numOfCells)
        {
            if (_gameMatrix.ColumnsSize * _gameMatrix.RowsSize < numOfCells)
                throw new OverflowException();
            var uniqueCombinations = new HashSet<MatrixCell>();
            var randomCellIndex = UnityEngine.Random.Range(0, _gameMatrix.AllCells.Length);
            var newCell = _gameMatrix.AllCells[randomCellIndex];
            uniqueCombinations.Add(newCell);
            while (uniqueCombinations.Count < numOfCells)
            {
                var lastCell = uniqueCombinations.Last();
                var array = UnityEngine.Random.value > 0.5f ? lastCell.Column.Cells : lastCell.Row.Cells;
                var filteredArray = array.Except(uniqueCombinations).ToArray();
                if (filteredArray.Length <= 0)
                {
                    array = array.Equals(lastCell.Column.Cells) ? lastCell.Row.Cells : lastCell.Column.Cells;
                    filteredArray =  array.Except(uniqueCombinations).ToArray();
                    if (filteredArray.Length <= 0)
                        throw new ArgumentException();
                }
                var newElement = array[UnityEngine.Random.Range(0, array.Length)];
                uniqueCombinations.Add(newElement);
            }
            
            return uniqueCombinations.Select(cell => cell.Value).ToArray();
        }
    }
}