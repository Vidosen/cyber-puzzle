using Prototype.Scripts.Matrix;

namespace Signals
{
    public static class MatrixSignals
    {
        public class VectorSwapRequest
        {
            public BaseVector ActiveVector { get; set; }
            public BaseVector PassiveVector { get; set; }
        }    }
}