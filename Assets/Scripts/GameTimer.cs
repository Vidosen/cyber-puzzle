using System;
using UniRx;

public class GameTimer
{
    private float _timeSpan;
    public bool IsTimeEnded => TimeLeft.Value <= 0;
    public IObservable<bool> IsTimeEndedObservable { get; }
    public ReactiveProperty<float> TimeLeft { get; } = new ReactiveProperty<float>();
    public float TimerDuration { get; private set; }

    private IDisposable _timerStream;
    
    public GameTimer(double timeSpan = 1f)
    {
        SetTimeSpan(timeSpan);
        IsTimeEndedObservable = TimeLeft.Select(timeLeft => timeLeft <= 0);
    }
    public GameTimer(double timerDuration, double timeSpan = 1f)
    {
        SetTimeSpan(timeSpan);
        SetDuration(timerDuration);
        IsTimeEndedObservable = TimeLeft.Select(timeLeft => timeLeft <= 0);
    }

    public void SetDuration(double timerDuration) => SetDuration((float) timerDuration);
    public void SetDuration(float timerDuration)
    {
        TimerDuration = timerDuration;
        TimeLeft.Value = timerDuration;
    }

    public void SetTimeSpan(double timeSpan) => SetTimeSpan((float) timeSpan);
    public void SetTimeSpan(float timeSpan)
    {
        _timeSpan = timeSpan;
    }
    public void Start()
    {
        _timerStream?.Dispose();
        TimeLeft.Value = TimerDuration;
        _timerStream = Observable.Interval(TimeSpan.FromSeconds(_timeSpan))
            .ObserveOnMainThread()
            .Subscribe(_ =>
        {
            TimeLeft.Value = Math.Max(0f, TimeLeft.Value - _timeSpan);
            if (IsTimeEnded)
                _timerStream.Dispose();
        });
    }
    public void Pause()
    {
        _timerStream?.Dispose();
    }
    
    public void Stop()
    {
        TimeLeft.Value = 0;
        _timerStream?.Dispose();
    }
}