using System;
using Minigames.MatrixBreaching.Matrix.Data;
using UniRx;

namespace Minigames.MatrixBreaching.Matrix.Interfaces
{
    public interface ICell : IDisposable
    {
        CellType CellType { get; }
        int HorizontalId { get; }
        int VerticalId { get; }
        IReadOnlyReactiveProperty<float> LockTimeLeft { get; }
        IReadOnlyReactiveProperty<bool> IsLocked { get; }

        void Lock(float lockTime);
        void Unlock();
        void Move(int horizontal, int vertical);
    }
}