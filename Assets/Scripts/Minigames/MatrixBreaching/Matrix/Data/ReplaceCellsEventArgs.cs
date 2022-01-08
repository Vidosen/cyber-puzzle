using Minigames.MatrixBreaching.Matrix.Interfaces;

namespace Minigames.MatrixBreaching.Matrix.Data
{
    public struct ReplaceCellsEventArgs
    {
        public ReplaceCellsEventArgs(ICell disposedCell, ICell newCell)
        {
            DisposedCell = disposedCell;
            NewCell = newCell;
        }

        public ICell DisposedCell { get; }
        public ICell NewCell { get; }
    }
}