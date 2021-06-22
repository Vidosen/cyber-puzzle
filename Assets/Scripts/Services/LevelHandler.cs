using System;
using Data;
using UniRx;
using UnityEngine;

namespace Services
{
    public class LevelHandler : MonoBehaviour
    {
        public readonly ReactiveProperty<LevelState> LevelStateReactive = new ReactiveProperty<LevelState>(LevelState.Pending);
        public GameTimer LevelTimer => _gameTimer;
        [SerializeField, Space] private GameObject winView;
        [SerializeField] private GameObject loseView;

        [SerializeField] private MatrixHandler _matrixHandler;
        [SerializeField] private CombinationsHandler _combinationsHandler;
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private GameTimer _gameTimer = new GameTimer();

        public void InitAndStartLevel(LevelSettings levelSettings)
        {
            winView.SetActive(false);
            loseView.SetActive(false);
            _matrixHandler.MatrixChanged.Subscribe(_ => OnMatrixChanged()).AddTo(_compositeDisposable);
            _gameTimer.Stop();
            _gameTimer.SetDuration(levelSettings.LevelTimer);
            _gameTimer.IsTimeEndedObservable.Where(isEnded => isEnded).Subscribe(_ => OnTimeEnded())
                .AddTo(_compositeDisposable);
            _gameTimer.Start();
            _combinationsHandler.InitCombinations(levelSettings);
            _matrixHandler.InitMatrix(levelSettings);
            LevelStateReactive.Value = LevelState.InProgress;
        }

        private void OnTimeEnded()
        {
            EndLevel(_combinationsHandler.AllCombinationsCompleted);
        }

        public void OnMatrixChanged()
        {
            _combinationsHandler.UpdateCombinations();
            
            if (_combinationsHandler.AllCombinationsCompleted)
                EndLevel(true);
        }
        public void StopAndDisposeLevel()
        {
            _matrixHandler.DisposeMatrix();
            _combinationsHandler.DisposeCombinations();
            _compositeDisposable.Clear();
            LevelStateReactive.Value = LevelState.Pending;
        }
        
        private void EndLevel(bool isWin)
        {
            winView.SetActive(isWin);
            loseView.SetActive(!isWin);
            
            _gameTimer.Pause();
            LevelStateReactive.Value = isWin ? LevelState.Won : LevelState.Lost;
        }
    }

    public enum LevelState
    {
        Pending,
        InProgress,
        Won,
        Lost
    }
}