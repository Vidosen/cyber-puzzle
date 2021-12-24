using Minigames.MatrixBreaching.Matrix.Data;

namespace Minigames.MatrixBreaching.Matrix.Models
{
    public class ValueCell : BaseCell
    {
        public CellValueType ValueType { get; }

        public ValueCell(CellValueType valueType)
        {
            ValueType = valueType;
        }
        public override void Dispose()
        {
        }
    }
}