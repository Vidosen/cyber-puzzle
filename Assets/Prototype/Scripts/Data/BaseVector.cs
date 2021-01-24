using ModestTree;
using UnityEngine;

namespace Prototype.Scripts.Data
{
    public abstract class BaseVector {

        private Cell[] cells;
        public int Size => cells.Length;
        
        public Cell this[int index]
        {
            get
            {
                if (index >= cells.Length)
                {
                    Debug.LogError("[Data] BaseVector.this[int index]: index is out of range");
                    return default;
                }
                return cells[index];
            }
            set
            {
                if (index >= cells.Length)
                    {
                        Debug.LogError("[Data] BaseVector.this[int index]: index is out of range");
                        return;
                    }
                cells[index] = value;
            }
        }

        public int IndexOfCell(Cell cell)
        {
            for (int i = 0; i < Size; i++)
            {
                if (cell.Equals(cells[i]))
                {
                    return i;
                }
            }
            return -1;
        }
        protected BaseVector(int cellsCount)
        {
            cells = new Cell[cellsCount];
        }
    }
}