using System;
using System.Collections.Generic;
using Prototype.Scripts.Data;
using Prototype.Scripts.Utils;
using Prototype.Scripts.Views;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Prototype.Scripts.Services
{
    
    public class GameService : IService
    {
        
        [Inject] private Canvas canvas;
        
        private LevelSO LevelPreset;

        private GameMatrix gameMatrixPrefab;
        private Combination combinationPrefab;
        
        private GameMatrix gameMatrix;
        private List<Combination> activeCombinations;

        public GameService(GameMatrix _gameMatrixPrefab, Combination _combination)
        {
            gameMatrixPrefab = _gameMatrixPrefab;
            combinationPrefab = _combination;
            LevelPreset = Resources.Load<LevelSO>("Levels/Level_1");
        }
        public void Initialize()
        {
            StartLevel();
        }

        void StartLevel()
        {
            gameMatrix = Object.Instantiate(gameMatrixPrefab, canvas.transform);
            if (!gameMatrix.IsInitialized)
                gameMatrix.InitializeFromLevelSO(LevelPreset);
        }
        
    }
}
