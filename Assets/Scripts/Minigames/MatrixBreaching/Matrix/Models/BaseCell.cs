
using System;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using UniRx;

namespace Minigames.MatrixBreaching.Matrix.Models
{
    public abstract class BaseCell : ICell
    {
        public abstract void Dispose();

        public IObservable<Unit> CellPositionChanged => _positionChangedSubject;
        protected Subject<Unit> _positionChangedSubject = new Subject<Unit>();
        public int HorizontalId { get; protected set; }
        public int VerticalId { get; protected set; }

        public virtual void Move(int horizontal, int vertical)
        {
            HorizontalId = horizontal;
            VerticalId = vertical;
            _positionChangedSubject.OnNext(Unit.Default);
        }
    }
}