using System;
using UniRx;

namespace Minigames.MatrixBreaching.Matrix.Interfaces
{
    public interface ICell : IDisposable
    {
        IObservable<Unit> CellUpdated { get; }

        int HorizontalId { get; }
        int VerticalId { get; }
        void Move(int horizontal, int vertical);
    }
}