using Minigames.MatrixBreaching.Matrix.Views;
using Minigames.MatrixBreaching.Matrix.Views.Cells;

namespace Minigames.MatrixBreaching.Matrix.Data
{
    public struct ReplaceCellViewsEventArgs
    {
        public ReplaceCellViewsEventArgs(BaseCellView disposedCellView, BaseCellView newCellView)
        {
            DisposedCellView = disposedCellView;
            NewCellView = newCellView;
        }

        public BaseCellView DisposedCellView { get; }
        public BaseCellView NewCellView { get; }
    }
}