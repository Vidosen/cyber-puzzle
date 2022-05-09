using Minigames.MatrixBreaching.Matrix.Data;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Models.Cells
{
    public class ShuffleCell : BaseCell
    {
        public ShuffleCell(SignalBus signalBus) : base(signalBus)
        { }
        public override CellType CellType => CellType.Shuffle;
    }
}