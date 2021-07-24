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
        [SerializeField] private ProgressHandler _progressHandler;

        private Tweener _progressAnimation;

        private void Start()
        {
            _progressHandler.ProgressInited.Subscribe(_ => UpdateGoalView()).AddTo(this);
            _progressHandler.ProgressUpdated.Subscribe(progress => UpdateProgressView(progress)).AddTo(this);
        }

        private void UpdateProgressView(float progress)
        {
            _progressAnimation?.Kill();
            _progressAnimation = _progressBarSlider.DOValue(progress, 0.3f).SetEase(Ease.InOutSine);
        }

        private void UpdateGoalView()
        {
            _goalText.text = _progressHandler.GoalProgress.ToString("0");
        }
    }
}
