using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Services;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

public class LevelTimerView : MonoBehaviour
{
    [SerializeField] private LevelHandler _levelHandler;
    [SerializeField] private Slider _timer;
    private Tween _timerTween;

    private void Start()
    {
        _levelHandler.LevelTimer.TimeLeft.Subscribe(_ => OnTimeChanged()).AddTo(this);
    }

    private void OnTimeChanged()
    {
        var timer = _levelHandler.LevelTimer;
        var relativeValue = Mathf.Clamp01((float)(timer.TimeLeft.Value / timer.TimerDuration));
        UpdateBar(relativeValue);
    }

    private void UpdateBar(float relativeValue)
    {
        _timerTween?.Kill();
        _timerTween = _timer.DOValue(relativeValue, 0.5f).SetEase(Ease.InQuad);
    }
}
