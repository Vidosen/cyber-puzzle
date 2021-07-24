using System;
using Data;
using Prototype.Scripts.Combinations;
using UniRx;
using UnityEngine;

namespace Services
{
    public class ProgressHandler : MonoBehaviour
    {
        [SerializeField] private LevelsManager _levelsManager;

        public float NormalizedProgress => Mathf.Clamp01(_currentProgress / _goalProgress);
        public bool IsProgressFilled => _currentProgress >= _goalProgress;

        public float GoalProgress => _goalProgress;

        private float _goalProgress;
        private float _currentProgress;
        private ILevelSettings _defaultlevelSettings;

        public Subject<float> ProgressUpdated = new Subject<float>();
        public Subject<Unit> ProgressInited = new Subject<Unit>();

        private void Awake()
        {
            _defaultlevelSettings = Resources.Load<GeneralGameplaySettings>("Settings/GeneralLevelSettings");
        }

        public void InitProgress(ILevelSettings levelSettings = null)
        {
            _goalProgress = levelSettings != null ? (float)levelSettings.HPRequired : CalculateGoalProgress();
            ProgressInited.OnNext(Unit.Default);
        }


        public void AddProgress(Combination combination)
        {
            _currentProgress += combination.HPForCombination;
            ProgressUpdated.OnNext(NormalizedProgress);
            
        }
        private float CalculateGoalProgress()
        {
            return (float) _defaultlevelSettings.HPRequired * (_levelsManager.CurrentLevel * 0.1f + 1f);
        }
        
        public void Reset()
        {
            _currentProgress = 0;
            _goalProgress = 0;
            ProgressUpdated.OnNext(NormalizedProgress);
        }

    }
}
