using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Services;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class ProgressView : MonoBehaviour
    {
        [SerializeField] private Slider _progressBarSlider;
        [SerializeField] private TextMeshProUGUI _goalText;
        [Space]
        [SerializeField] private LevelProgressHandler levelProgressHandler;

        private Tweener _progressAnimation;

        private void Start()
        {
            levelProgressHandler.ProgressInited.Subscribe(_ => UpdateGoalView()).AddTo(this);
            levelProgressHandler.ProgressUpdated.Subscribe(progress => UpdateProgressView(progress)).AddTo(this);
        }

        private void UpdateProgressView(float progress)
        {
            if (_progressAnimation.IsActive())
                _progressAnimation.Kill();
            _progressAnimation = _progressBarSlider.DOValue(progress, 0.3f).SetEase(Ease.InOutSine);
        }

        private void UpdateGoalView()
        {
            _goalText.text = levelProgressHandler.GoalProgress.ToString("0");
        }
    }
}
