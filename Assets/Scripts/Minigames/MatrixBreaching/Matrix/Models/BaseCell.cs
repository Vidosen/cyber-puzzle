
using System;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Signals;
using UniRx;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Models
{
    public abstract class BaseCell : ICell
    {
        private readonly SignalBus _signalBus;
        public IObservable<Unit> CellPositionChanged => _positionChangedSubject;
        protected Subject<Unit> _positionChangedSubject = new Subject<Unit>();
        public int HorizontalId { get; protected set; }
        public int VerticalId { get; protected set; }
        public BaseCell(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public virtual void Move(int horizontal, int vertical)
        {
            HorizontalId = horizontal;
            VerticalId = vertical;
            _positionChangedSubject.OnNext(Unit.Default);
        }
        public virtual void Dispose()
        {
            _signalBus.Fire(new MatrixSignals.CellDisposed(this));
        }
    }
}