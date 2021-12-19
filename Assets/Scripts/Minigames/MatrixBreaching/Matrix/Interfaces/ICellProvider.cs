using System.Collections.Generic;

namespace Minigames.MatrixBreaching.Matrix.Interfaces
{
    public interface ICellProvider
    {
        IEnumerable<ICell> GetNewCells(int size);
        ICell GetNewCell();
    }
}
