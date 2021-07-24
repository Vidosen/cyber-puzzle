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
        [SerializeField] private ProgressHandler _progressHandler;
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private GameTimer _gameTimer = new GameTimer();

        public void InitAndStartLevel(LevelSettings levelSettings)
        {
            UpdateView();

            _gameTimer.Stop();
            _gameTimer.SetDuration(levelSettings.LevelTimer);
            _gameTimer.IsTimeEndedObservable.Where(isEnded => isEnded).Subscribe(_ => OnTimeEnded())
                .AddTo(_compositeDisposable);
            _gameTimer.Start();
            _matrixHandler.InitMatrix(levelSettings);
            _combinationsHandler.InitCombinations(levelSettings);
            _progressHandler.InitProgress();
            
            _matrixHandler.MatrixChanged
                .Subscribe(_ => OnMatrixChanged())
                .AddTo(_compositeDisposable);
            
            _progressHandler.ProgressUpdated
                .Where(_ => _progressHandler.IsProgressFilled)
                .Subscribe(_ => EndLevel(true))
                .AddTo(_compositeDisposable);
            
            OnMatrixChanged();
            LevelStateReactive.Value = LevelState.InProgress;
        }

        private void UpdateView()
        {
            winView.SetActive(false);
            loseView.SetActive(false);
        }

        private void OnTimeEnded()
        {
            EndLevel(false);
        }

        public void OnMatrixChanged()
        {
            _combinationsHandler.UpdateCombinations();
        }
        public void StopAndDisposeLevel()
        {
            _matrixHandler.DisposeMatrix();
            _combinationsHandler.ResetCombinations();
            _progressHandler.Reset();
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