using Combinations;

namespace Signals
{
    public static class CombinationSignals
    {
        public abstract class BaseCombinationCellSignal
        {
            public CombinationCell Cell { get; set; }
            public Combination Combination { get; set; }
        }
        public class HoverCellEnter : BaseCombinationCellSignal
        {
        }
        public class HoverCellExit : BaseCombinationCellSignal
        {
        }
    }
}