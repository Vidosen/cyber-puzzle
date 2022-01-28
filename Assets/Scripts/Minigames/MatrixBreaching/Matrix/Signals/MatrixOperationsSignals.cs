using System.Collections.Generic;
using System.Linq;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;

namespace Minigames.MatrixBreaching.Matrix.Signals
{
    public static class MatrixOperationsSignals
    {
        public interface IOperationSignal
        {
            OperationType OperationType { get; }
        }

        public class PostOperationOccured : IOperationSignal
        {
            public PostOperationOccured(OperationType operationType)
            {
                OperationType = operationType;
            }

            public OperationType OperationType { get; }
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
        
        public class ScrollOperationOccured : IOperationSignal
        {
            public OperationType OperationType => OperationType.Scroll;
            public RowType RowType { get; private set; }
            public int RowIndex { get; private set; }

            public ScrollOperationOccured(RowType rowType, int rowIndex)
            {
                RowType = rowType;
                RowIndex = rowIndex;
            }
        }

        public class OperationApplied
        {
            public List<ICell> InvolvedCells { get; }

            public OperationApplied(params ICell[] involvedCells)
            {
                InvolvedCells = involvedCells.ToList();
            }
        }
    }
}