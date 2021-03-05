using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ModestTree;
using Prototype.Scripts.Data;
using Prototype.Scripts.Views;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Prototype.Scripts.Services
{
    
    public class GameService : Singleton<GameService>
    {
        [SerializeField] private Transform matrixConatainer;
        [SerializeField] private Transform combinationsContainer;
        [SerializeField] private GameMatrix gameMatrixPrefab;
        [SerializeField] private Combination combinationPrefab;
        
        private LevelSO LevelPreset;

        private GameMatrix gameMatrix;
        private List<Combination> combinations = new List<Combination>();

        public event Action MatrixChanged;

        private void Awake()
        {
            LevelPreset = Resources.Load<LevelSO>("Levels/Level_1");
            MatrixChanged += CheckCombinations;
            //MatrixChanged += DebugMatrix;
        }

        private void DebugMatrix()
        {
            Debug.Log("NEW MATRIX:\b");
            for (int i = 0; i < gameMatrix.RowsSize; i++)
            {
                var resultStr = "| ";
                for (int j = 0; j < gameMatrix.ColumnsSize; j++)
                {
                    var pos = (j , i);
                    resultStr += $"({pos.Item1},{pos.Item2})" + gameMatrix[j, i].Value;
                    if (j == gameMatrix.ColumnsSize - 1)
                    {
                        resultStr+= " |\b\b";
                        continue;
                    }
                    resultStr+= " | ";
                }
                Debug.Log(resultStr);
            }
        }

        public void Start()
        {
            StartLevel();
            MatrixChanged?.Invoke();
        }

        void StartLevel()
        {
            gameMatrix = Instantiate(gameMatrixPrefab, matrixConatainer);
            gameMatrix.ThisTransform.localPosition = Vector2.zero;
            if (!gameMatrix.IsInitialized)
                gameMatrix.InitializeFromLevelSO(LevelPreset);

            foreach (var codeCombination in LevelPreset.CodeCombinations)
            {
                var combination = Instantiate(combinationPrefab, combinationsContainer);
                combination.Initialize(codeCombination);
                combinations.Add(combination);   
            }
        }

        #region Highlight Methods
        public void HighlightAllMatchingMatrixCells(int compValue, Color highlightColor, HighlightType type)
        {
            foreach (var matrixCell in gameMatrix.AllCells.Where(cell => cell.Value == compValue))
                matrixCell.HighlightCell(highlightColor, type);
        }
        private void DimAllCombinationCells(HighlightType type)
        {
            foreach (var matrixCell in combinations.SelectMany(s=>s.CombinationCodes))
                matrixCell.DimCell(type);
        }
        public void DimAllMatrixCells(HighlightType type)
        {
            foreach (var matrixCell in gameMatrix.AllCells)
                matrixCell.DimCell(type);
        }
        #endregion

        public void SwapRequest(BaseVector oneVector, BaseVector twoVector)
        {
            gameMatrix.SwapVectors(oneVector, twoVector);
            MatrixChanged?.Invoke();
        }

        public void CheckCombinations()
        {
            DimAllMatrixCells(HighlightType.CombinationSequence);
            DimAllCombinationCells(HighlightType.CombinationSequence);
            foreach (var combination in combinations)
                HighlightCombination(combination, 2);
        }

        private void HighlightCombination(Combination combination, int minCount)
        {
            var foundCodes= FindBestMatrixCombination(combination.CombinationCodes);
            if (foundCodes.Count >= minCount)
            {
                for (int i = 0; i < Mathf.Min(foundCodes.Count, combination.CombinationCodes.Count); i++)
                {
                    foundCodes[i].HighlightCell(combination.HighlightColor, HighlightType.CombinationSequence);
                    combination.CombinationCodes[i].HighlightCell(combination.HighlightColor, HighlightType.CombinationSequence);
                }
            }
            combination.ChangeIsCombinationComplete(foundCodes.Count == combination.CombinationCodes.Count);
        }

        private List<MatrixCell> FindBestMatrixCombination(List<CombinationCell> codes)
        {
            List<List<MatrixCell>> foundCombinations = new List<List<MatrixCell>>();
            var firstCell = codes.FirstOrDefault();
            if (firstCell == null)
                throw new Exception("'codes' parameter doesn't contain any CombinationCell!");
            var startCells = gameMatrix.AllCells.Where(mCell => mCell.Value == firstCell.Value);
            foreach (var cell in startCells)
            {
                List<MatrixCell> result = new List<MatrixCell>();
                FindMatrixCombination(cell, codes, 1, ref result);
                foundCombinations.Add(result);
            }
            return foundCombinations.Max();
        }

        private void FindMatrixCombination(MatrixCell cell, List<CombinationCell> codes, int currentIndex, ref List<MatrixCell> resultCells)
        {
            if (resultCells == null) resultCells = new List<MatrixCell>();
            resultCells.Add(cell);
            if (currentIndex >= codes.Count)
                return;

            var pos = gameMatrix.IndexOf(cell);
            var i = pos.Item1;
            var j = pos.Item2;

            if (i + 1 < gameMatrix.ColumnsSize && gameMatrix[i + 1, j].Value == codes[currentIndex].Value)
            {
                FindMatrixCombination(gameMatrix[i + 1, j], codes, ++currentIndex, ref resultCells);
                return;
            }
            if (i - 1 >= 0 && gameMatrix[i - 1, j].Value == codes[currentIndex].Value)
            {
                FindMatrixCombination(gameMatrix[i - 1, j], codes, ++currentIndex, ref resultCells);
                return;
            }

            if (j + 1 < gameMatrix.RowsSize && gameMatrix[i, j + 1].Value == codes[currentIndex].Value)
            {
                FindMatrixCombination(gameMatrix[i, j + 1], codes, ++currentIndex, ref resultCells);
                return;
            }

            if (j - 1 >= 0 && gameMatrix[i, j - 1].Value == codes[currentIndex].Value)
            {
                FindMatrixCombination(gameMatrix[i, j - 1], codes, ++currentIndex, ref resultCells);
                return;
            }
        }
    }
}
