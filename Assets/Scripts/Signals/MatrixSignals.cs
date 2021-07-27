using Matrix;

namespace Signals
{
    public static class MatrixSignals
    {
        public class VectorSwapRequest
        {
            public BaseVector ActiveVector { get; set; }
            public BaseVector PassiveVector { get; set; }
        }
        
        public class VectorScrollRequest
        {
            public BaseVector ActiveVector { get; set; }
            public int SlideOffset { get; set; }
        }
    }
}