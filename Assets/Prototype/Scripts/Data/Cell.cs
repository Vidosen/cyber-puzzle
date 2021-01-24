namespace Prototype.Scripts.Data
{
    public class Cell
    {
        public RowVector Row { get; protected set; }
        public ColumnVector Column { get; protected set; }
        public int Value { get; set; }

        public Cell(RowVector  row, ColumnVector column)
        {
            Row = row;
            Column = column;
        }
    }
}