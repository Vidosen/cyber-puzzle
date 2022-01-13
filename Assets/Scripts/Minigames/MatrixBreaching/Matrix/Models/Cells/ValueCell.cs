using Minigames.MatrixBreaching.Matrix.Data;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Models.Cells
{
    public class ValueCell : BaseCell
    {
        public CellValueType Value { get; protected set; }
        public override CellType CellType => CellType.Value;

        public ValueCell(CellValueType value, SignalBus signalBus) : base(signalBus)
        {
            Value = value;
        }
    }
}