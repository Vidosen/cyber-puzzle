using Minigames.MatrixBreaching.Matrix.Interfaces;

namespace Minigames.MatrixBreaching.Matrix.Operations.Commands
{
    public abstract class BaseRowScrollCommand : IMatrixCommand
    {
        public int RowId { get; protected set;}
        public int ScrollDelta { get; protected set;}
        public virtual void Initialize(int rowId, int scrollDelta)
        {
            RowId = rowId;
            ScrollDelta = scrollDelta;
        }
        public virtual void Execute()
        {
            ScrollRow(false);
        }

        protected abstract void ScrollRow(bool isReversed);

        public virtual void Cancel()
        {
            ScrollRow(true);
        }
    }
}