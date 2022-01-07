using Minigames.MatrixBreaching.Matrix.Data;

namespace Minigames.MatrixBreaching.Matrix.Models
{
    public class ValueCell : BaseCell
    {
        public CellValueType Value { get; }

        public ValueCell(CellValueType value)
        {
            Value = value;
        }
        public override void Dispose()
        {
        }
    }
}