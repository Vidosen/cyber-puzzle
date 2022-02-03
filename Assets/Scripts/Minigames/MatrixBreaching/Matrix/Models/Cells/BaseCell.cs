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
        public int HorizontalId { get; protected set; }
        public int VerticalId { get; protected set; }
        public bool IsDisposed { get; protected set; }
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
            IsDisposed = true;
            _signalBus.Fire(new MatrixSignals.CellDisposed(this));
        }

        public override string ToString()
        {
            return $"{GetType().Name} ({HorizontalId}, {VerticalId}) Hash: {GetHashCode()}";
        }
    }
}