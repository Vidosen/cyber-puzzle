using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Matrix;
using Prototype.Scripts.Data;
using Prototype.Scripts.Matrix;
using Prototype.Scripts.Providers.Mono;
using UniRx;
using UnityEngine;

namespace Services
{
    public class MatrixHandler : MonoBehaviour
    {
        [SerializeField] private MatrixProvider _matrixProvider;
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

            _gameMatrix.AnyVectorSwapRequest
                .Subscribe(o => SwapVectors(o.Item1, o.Item2))
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
            _gameMatrix?.Dispose();
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

        private void FindMatrixCombination(MatrixCell cell, List<ICell> combination, int currentIndex,
            ref List<MatrixCell> resultCells)
        {
            if (resultCells == null) resultCells = new List<MatrixCell>();
            resultCells.Add(cell);
            if (currentIndex >= combination.Count)
                return;

            var pos = _gameMatrix.IndexOf(cell);
            var i = pos.Item1;
            var j = pos.Item2;

            if (i + 1 < _gameMatrix.ColumnsSize && _gameMatrix[i + 1, j].Value == combination[currentIndex].Value)
            {
                FindMatrixCombination(_gameMatrix[i + 1, j], combination, ++currentIndex, ref resultCells);
                return;
            }

            if (i - 1 >= 0 && _gameMatrix[i - 1, j].Value == combination[currentIndex].Value)
            {
                FindMatrixCombination(_gameMatrix[i - 1, j], combination, ++currentIndex, ref resultCells);
                return;
            }

            if (j + 1 < _gameMatrix.RowsSize && _gameMatrix[i, j + 1].Value == combination[currentIndex].Value)
            {
                FindMatrixCombination(_gameMatrix[i, j + 1], combination, ++currentIndex, ref resultCells);
                return;
            }

            if (j - 1 >= 0 && _gameMatrix[i, j - 1].Value == combination[currentIndex].Value)
            {
                FindMatrixCombination(_gameMatrix[i, j - 1], combination, ++currentIndex, ref resultCells);
            }
        }
    }
}