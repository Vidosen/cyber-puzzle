using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prototype.Scripts.Combinations;
using Prototype.Scripts.Data;
using Prototype.Scripts.Matrix;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Scripts.Services
{
    
    public class GameService : Singleton<GameService>
    {
        [SerializeField] private Transform matrixConatainer;
        [SerializeField] private Transform combinationsContainer;
        [SerializeField] private GameMatrix gameMatrixPrefab;
        [SerializeField] private Combination combinationPrefab;
        [SerializeField, Space] private GameObject winView;
        [SerializeField] private GameObject loseView;
        [SerializeField] private Slider timerSlider;
        
        private LevelSO[] Levels;

        private int indexLevel = 0;
        private LevelSO CurrentLevel;

        private GameMatrix gameMatrix;
        private List<Combination> combinations = new List<Combination>();

        public bool AllCombinationsCompleted => combinations.All(c => c.IsCombinationComplete);

        private IEnumerator _timerRoutine;
        private float _timeLeft;

        public event Action MatrixChanged;

        private void Awake()
        {
            Levels = Resources.LoadAll<LevelSO>("Levels/");
            
            CurrentLevel = Levels[indexLevel];
            MatrixChanged += CheckCombinations;
#if DEBUG
            CurrentLevel = Levels.LastOrDefault();
            MatrixChanged += DebugMatrix;      
#endif
        }
        

        IEnumerator StartTimer()
        {
            var delta = 0.1f;
            var secondDelay = new WaitForSeconds(delta);
            _timeLeft = CurrentLevel.LevelTimer;
            while (_timeLeft > 0)
            {
                timerSlider.value = Mathf.Max(0, _timeLeft / CurrentLevel.LevelTimer);
                _timeLeft -= delta;
                yield return secondDelay;
            }
            timerSlider.value = Mathf.Max(0, _timeLeft / CurrentLevel.LevelTimer);
            EndLevel(AllCombinationsCompleted);
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
        }

        public void StartLevel()
        {
            winView.SetActive(false);
            loseView.SetActive(false);
            
            InitMatrix();
            InitCombinations();
            
            StartCoroutine(_timerRoutine = StartTimer());
            MatrixChanged?.Invoke();
        }

        public void NextLevel()
        {
            indexLevel = indexLevel + 1 < Levels.Length ? indexLevel + 1 : 0;
            CurrentLevel = Levels[indexLevel];
            RestartLevel();
        }

        private void InitMatrix()
        {
            gameMatrix = Instantiate(gameMatrixPrefab, matrixConatainer);
            gameMatrix.ThisTransform.localPosition = Vector2.zero;
            if (!gameMatrix.IsInitialized)
                gameMatrix.InitializeFromLevelSO(CurrentLevel);
        }

        private void InitCombinations()
        {
            foreach (var codeCombination in CurrentLevel.CodeCombinations)
            {
                var combination = Instantiate(combinationPrefab, combinationsContainer);
                combination.Initialize(codeCombination);
                combinations.Add(combination);
            }
        }

        public void RestartLevel()
        {
            ResetGame();
            StartLevel();
        }
        
        public void ResetGame()
        {
            gameMatrix?.Dispose();
            gameMatrix = null;
            combinations?.ForEach(c => c.Dispose());
            combinations?.Clear();
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
                CheckCombination(combination, 2);
            
            if (AllCombinationsCompleted)
                EndLevel(true);
        }

        private void EndLevel(bool isWin)
        {
            if (_timerRoutine != null)
                StopCoroutine(_timerRoutine);
            winView.SetActive(isWin);
            loseView.SetActive(!isWin);
        }

        private void CheckCombination(Combination combination, int minCount)
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
            return foundCombinations
                .OrderByDescending(c => c.Count)
                .FirstOrDefault();
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
