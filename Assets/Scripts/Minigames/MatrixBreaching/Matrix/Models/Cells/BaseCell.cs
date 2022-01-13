using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Signals;
using UniRx;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Models.Cells
{
    public abstract class BaseCell : ICell
    {
        private readonly SignalBus _signalBus;
        private ReactiveProperty<float> _lockTimeLeft = new ReactiveProperty<float>();
        public int HorizontalId { get; protected set; }
        public int VerticalId { get; protected set; }
        public IReadOnlyReactiveProperty<float> LockTimeLeft => _lockTimeLeft;
        public IReadOnlyReactiveProperty<bool> IsLocked => _lockTimeLeft.Select(timeLeft => timeLeft > 0).ToReadOnlyReactiveProperty();
        public void Lock(float lockTime)
        {
            _lockTimeLeft.Value = lockTime;
        }
        public void Unlock()
        {
            _lockTimeLeft.Value = 0;
        }

        public abstract CellType CellType { get; }
        public BaseCell(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public virtual void Move(int horizontal, int vertical)
        {
            HorizontalId = horizontal;
            VerticalId = vertical;
            _signalBus.Fire(new MatrixSignals.CellMoved(this));
        }
        public virtual void Dispose()
        {
            _signalBus.Fire(new MatrixSignals.CellDisposed(this));
            HorizontalId = -1;
            VerticalId = -1;
        }

        public override string ToString()
        {
            return $"{GetType().Name} ({HorizontalId}, {VerticalId}) Hash: {GetHashCode()}";
        }
    }
}