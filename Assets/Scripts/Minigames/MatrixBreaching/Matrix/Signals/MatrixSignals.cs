using Minigames.MatrixBreaching.Matrix.Interfaces;

namespace Minigames.MatrixBreaching.Matrix.Signals
{
    public static class MatrixSignals
    {
        public class CellDisposed
        {
            public ICell DisposedCell { get; }

            public CellDisposed(ICell cell)
            {
                DisposedCell = cell;
            }
        }
    }
}