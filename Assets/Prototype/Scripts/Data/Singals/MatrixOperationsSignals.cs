namespace Prototype.Scripts.Data.Singals
{
    public static class MatrixOperationsSignals
    {
        public abstract class BaseSwapSignal
        {
            public int FirstVectorIndex;
            public int SecondVectorIndex;
            public BaseSwapSignal(int firstIndex, int secondIndex)
            {
                FirstVectorIndex = firstIndex;
                SecondVectorIndex = secondIndex;
            }
        }
        
        public class ColumnsSwaped : BaseSwapSignal
        {
            public ColumnsSwaped(int firstIndex, int secondIndex) : base(firstIndex, secondIndex)
            { }
        }
        public class RowsSwaped : BaseSwapSignal
        {
            public RowsSwaped(int firstIndex, int secondIndex) : base(firstIndex, secondIndex)
            { }
        }

        public class RowsSwapRequest : BaseSwapSignal
        {
            public RowsSwapRequest(int firstIndex, int secondIndex) : base(firstIndex, secondIndex)
            { }
        }
        public class ColumnsSwapRequest : BaseSwapSignal
        {
            public ColumnsSwapRequest(int firstIndex, int secondIndex) : base(firstIndex, secondIndex)
            { }
        }
    }
}
