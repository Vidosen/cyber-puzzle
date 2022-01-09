using Minigames.MatrixBreaching.Matrix.Views;

namespace Minigames.MatrixBreaching.Matrix.Data
{
    public struct ReplaceCellViewsEventArgs
    {
        public ReplaceCellViewsEventArgs(ValueCellView disposedCellView, ValueCellView newCellView)
        {
            DisposedCellView = disposedCellView;
            NewCellView = newCellView;
        }

        public ValueCellView DisposedCellView { get; }
        public ValueCellView NewCellView { get; }
    }
}