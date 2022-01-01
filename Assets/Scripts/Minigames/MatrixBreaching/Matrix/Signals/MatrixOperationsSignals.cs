using Minigames.MatrixBreaching.Matrix.Data;

namespace Minigames.MatrixBreaching.Matrix.Signals
{
    public static class MatrixOperationsSignals
    {
        public interface IOperationSignal
        {
            OperationType OperationType { get; }
        }
        public class SwapOperationOccured : IOperationSignal
        {
            public OperationType OperationType => OperationType.Swap;
            public RowType RowType { get; private set; }
            public int ApplyingRowIndex { get; private set; }
            public int AppliedRowIndex { get; private set; }

            public SwapOperationOccured(RowType rowType, int applyingRowIndex, int appliedRowIndex)
            {
                RowType = rowType;
                ApplyingRowIndex = applyingRowIndex;
                AppliedRowIndex = appliedRowIndex;
            }
        }
        
    }
}