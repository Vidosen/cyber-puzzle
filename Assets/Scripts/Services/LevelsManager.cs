using System;
using Data;
using UnityEngine;

namespace Services
{
    public class LevelsManager : MonoBehaviour
    {
        [SerializeField] private LevelHandler _levelHandler;
        private LevelSettings[] _levelSettings;
        private int indexLevel = 0;
        public LevelSettings CurrentLevel { get; private set; }

        private void Awake()
        {
            
            _levelSettings = Resources.LoadAll<LevelSettings>("Levels/");
            CurrentLevel = _levelSettings[indexLevel];
        }

        private void Start()
        {
            _levelHandler.InitAndStartLevel(CurrentLevel);
        }
        
        public void RestartLevel()
        {
            _levelHandler.StopAndDisposeLevel();
            _levelHandler.InitAndStartLevel(CurrentLevel);
        }
        public void NextLevel()
        {
            indexLevel = indexLevel + 1 < _levelSettings.Length ? indexLevel + 1 : 0;
            CurrentLevel = _levelSettings[indexLevel];
        }
    }
}