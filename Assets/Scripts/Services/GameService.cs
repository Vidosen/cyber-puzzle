using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prototype.Scripts.Combinations;
using Prototype.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Scripts.Services
{
    
    public class GameService : Singleton<GameService>
    {
        [SerializeField, Space] private GameObject winView;
        [SerializeField] private GameObject loseView;
        [SerializeField] private Slider timerSlider;
        [Space]
        [SerializeField] private MatrixHandler _matrixHandler;
        [SerializeField] private CombinationsHandler _combinationsHandler;
        
        private LevelSO[] Levels;

        private int indexLevel = 0;
        private LevelSO CurrentLevel;

        private IEnumerator _timerRoutine;
        private float _timeLeft;

        private void Awake()
        {
            
            Levels = Resources.LoadAll<LevelSO>("Levels/");
            
            CurrentLevel = Levels[indexLevel];
            _matrixHandler.MatrixChanged += OnMatrixChanged;
#if DEBUG
            CurrentLevel = Levels.LastOrDefault();
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
            EndLevel(_combinationsHandler.AllCombinationsCompleted);
        }

        public void Start()
        {
            StartLevel();
        }

        public void StartLevel()
        {
            winView.SetActive(false);
            loseView.SetActive(false);
            
            _combinationsHandler.InitCombinations(CurrentLevel);
            _matrixHandler.InitMatrix(CurrentLevel);
            
            StartCoroutine(_timerRoutine = StartTimer());
        }

        public void NextLevel()
        {
            indexLevel = indexLevel + 1 < Levels.Length ? indexLevel + 1 : 0;
            CurrentLevel = Levels[indexLevel];
            RestartLevel();
        }

        public void RestartLevel()
        {
            ResetGame();
            StartLevel();
        }
        
        public void ResetGame()
        {
            _matrixHandler.DisposeMatrix();
            _combinationsHandler.DisposeCombinations();
        }

        public void OnMatrixChanged()
        {
            _matrixHandler.DimAllMatrixCells(HighlightType.CombinationSequence);
            _combinationsHandler.DimAllCombinationCells(HighlightType.CombinationSequence);

            _combinationsHandler.UpdateCombinations();
            
            if (_combinationsHandler.AllCombinationsCompleted)
                EndLevel(true);
        }

        private void EndLevel(bool isWin)
        {
            if (_timerRoutine != null)
                StopCoroutine(_timerRoutine);
            winView.SetActive(isWin);
            loseView.SetActive(!isWin);
        }
    }
}
