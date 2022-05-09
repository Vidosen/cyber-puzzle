using System;
using Minigames.MatrixBreaching.Core.Data;
using UniRx;
using UnityEngine;

namespace Minigames.MatrixBreaching.Core
{
    public class MatrixBreachingModel
    {
        public IObservable<float> ProgressChanged => _progressChangedSubject;
        private Subject<float> _progressChangedSubject = new Subject<float>();
        public MatrixBreachingData.Settings Settings { get; private set; }

        public long CurrentProgress { get; private set; }
        public float CurrentRelativeProgress => TargetProgress > 0 ? Mathf.Clamp01((float) CurrentProgress / TargetProgress) : 0;
        public long TargetProgress => Settings.TargetProgress;

        public void Initialize(MatrixBreachingData.Settings setting)
        {
            Settings = setting;
            _progressChangedSubject.OnNext(CurrentProgress);
        }

        public void AddProgress(long deltaProgress)
        {
            CurrentProgress += deltaProgress;
            _progressChangedSubject.OnNext(CurrentProgress);
        }

        public void Reset()
        {
            CurrentProgress = 0;
            _progressChangedSubject.OnNext(CurrentProgress);
        }
    }
}