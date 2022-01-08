using Minigames.MatrixBreaching.Matrix.Interfaces;

namespace Minigames.MatrixBreaching.Matrix.Commands
{
    public abstract class BaseRowsSwapCommand : IMatrixCommand
    {
        public int ApplyingRowId { get; protected set;}
        public int AppliedRowId { get; protected set;}
        
        public void Initialize(int applyingRowId, int appliedRowId)
        {
            ApplyingRowId = applyingRowId;
            AppliedRowId = appliedRowId;
        }

        public abstract void Execute();

        public virtual void Cancel()
        {
            Execute();
        }
    }
}