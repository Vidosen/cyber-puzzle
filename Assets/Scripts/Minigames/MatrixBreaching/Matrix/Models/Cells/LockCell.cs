using Minigames.MatrixBreaching.Matrix.Data;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Models.Cells
{
    public class LockCell : BaseCell
    {
        public LockCell(SignalBus signalBus) : base(signalBus)
        { }
        public override CellType CellType => CellType.Lock;
    }
}