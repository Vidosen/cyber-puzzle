using System;
using UniRx;

namespace Minigames.MatrixBreaching.Matrix.Interfaces
{
    public interface ICell : IDisposable
    {
        IObservable<Unit> CellPositionChanged { get; }

        int HorizontalId { get; }
        int VerticalId { get; }
        void Move(int horizontal, int vertical);
    }
}