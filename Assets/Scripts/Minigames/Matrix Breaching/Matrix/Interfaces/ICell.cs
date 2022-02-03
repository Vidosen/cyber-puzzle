using System;
using Minigames.MatrixBreaching.Matrix.Data;

namespace Minigames.MatrixBreaching.Matrix.Interfaces
{
    public interface ICell : IDisposable
    {
        CellType CellType { get; }
        int HorizontalId { get; }
        int VerticalId { get; }
        bool IsDisposed { get; }
        void Move(int horizontal, int vertical);
    }
}