using System;
using System.Collections;
using Data;
using UnityEngine;

namespace Services
{
    public class LevelsManager : MonoBehaviour
    {
        [SerializeField] private LevelHandler _levelHandler;
        private LevelSettings[] _levelSettings;
        private int indexLevel;
        public int CurrentLevel { get; private set; }
        public LevelSettings CurrentLevelSettings { get; private set; }

        public int LevelIndex => indexLevel;

        private IEnumerator Start()
        {
            yield return _levelSettings = Resources.LoadAll<LevelSettings>("Levels/");
            CurrentLevelSettings = _levelSettings[indexLevel];
            _levelHandler.InitAndStartLevel(CurrentLevelSettings);
        }

        public void RestartLevel()
        {
            _levelHandler.StopAndDisposeLevel();
            _levelHandler.InitAndStartLevel(CurrentLevelSettings);
        }
        public void NextLevel()
        {
            indexLevel = indexLevel + 1 < _levelSettings.Length ? indexLevel + 1 : 0;
            CurrentLevel++;
            CurrentLevelSettings = _levelSettings[indexLevel];
        }
    }
}