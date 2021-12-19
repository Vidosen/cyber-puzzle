using Minigames.MatrixBreaching.Matrix.Data;

namespace Minigames.MatrixBreaching.Matrix.Models
{
    public class ValueCell : BaseCell
    {
        public CellValue Value { get; }

        public ValueCell(CellValue value)
        {
            Value = value;
        }
        public override void Dispose()
        {
        }
    }
}