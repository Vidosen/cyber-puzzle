using System;
using System.Collections.Generic;
using System.Linq;
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


        private void Awake()
        {
            LevelPreset = Resources.Load<LevelSO>("Levels/Level_1");
        }

        public void Start()
        {
            StartLevel();
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

        public void HighlightAllMatchingMatrixCells(int compValue, Color highlightColor)
        {
            foreach (var matrixCell in gameMatrix.AllCells.Where(cell => cell.Value == compValue))
                matrixCell.HighlightCell(highlightColor);
        }

        public void DimAllMatrixCells(Color dimColor)
        {
            foreach (var matrixCell in gameMatrix.AllCells)
                matrixCell.DimCell(dimColor);
        }
        
    }
}
