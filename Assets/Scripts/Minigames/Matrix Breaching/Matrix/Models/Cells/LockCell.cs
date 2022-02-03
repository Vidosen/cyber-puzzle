using System;
using Minigames.MatrixBreaching.Matrix.Data;
using UniRx;
using UnityEngine;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Models.Cells
{
    public class LockCell : BaseCell
    {
        public LockCell(SignalBus signalBus) : base(signalBus)
        { }
        public override CellType CellType => CellType.Lock;
        public IReadOnlyReactiveProperty<float> LockTimeLeft => _lockTimeLeft;
        public IReadOnlyReactiveProperty<bool> IsLocked => _lockTimeLeft
            .Select(timeLeft => timeLeft > 0)
            .ToReadOnlyReactiveProperty();

        public CompositeDisposable CellDisposable { get; } = new CompositeDisposable();
        
        private ReactiveProperty<float> _lockTimeLeft = new ReactiveProperty<float>();

        public void ApplyLock(float lockTime, float fixedStep = 0.05f)
        {
            _lockTimeLeft.Value = lockTime;
            Observable.Interval(TimeSpan.FromSeconds(fixedStep)).Subscribe(_ =>
            {
                _lockTimeLeft.Value = Mathf.Max(0, _lockTimeLeft.Value - fixedStep);
            }).AddTo(CellDisposable);
        }

        public override void Dispose()
        {
            base.Dispose();
            CellDisposable.Clear();
        }
    }
}