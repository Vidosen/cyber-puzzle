using System;
using UnityEngine;

namespace Prototype.Scripts.Data
{
    public class GameMatrix
    {
        private RowVector[] rowVectors;
        private ColumnVector[] columnVectors;

        public int VerticalSize => rowVectors.Length;
        public int HorizonalSize => columnVectors.Length;

        public GameMatrix(int rows, int columns)
        {
            GenerateVectors(rows, columns);
            GenerateCells();
        }

        private void GenerateVectors(int rows, int columns)
        {
            rowVectors = new RowVector[rows];
            columnVectors = new ColumnVector[columns];

            for (int i = 0; i < rowVectors.Length; i++)
                rowVectors[i] = new RowVector(columns);

            for (int i = 0; i < columnVectors.Length; i++)
                columnVectors[i] = new ColumnVector(rows);
        }

        private void GenerateCells()
        {
            for (int row = 0; row < VerticalSize; row++)
            {
                for (int column = 0; column < HorizonalSize; column++)
                {
                    var newCell = new Cell(rowVectors[row], columnVectors[column]);
                    rowVectors[row][column] = newCell;
                    columnVectors[column][row] = newCell;
                }
            }
        }

        public Cell this[int rowIndex, int columnIndex] => GetCellByRowColumnIndices(rowIndex, columnIndex);

        public Cell GetCellByRowColumnIndices(int rowIndex, int columnIndex)
        {
            if (rowIndex >= VerticalSize)
            {
                Debug.LogError("[Data] GameMatrix.GetCellByRowColumnIndices: rowIndex is out of range");
                return default;
            }
            return rowVectors[rowIndex][columnIndex];
        }

        public void SwapRows(int firstIndex, int secondIndex)
        {
            SwapVectors(rowVectors, firstIndex, secondIndex);
        }
        public void SwapColumns(int firstIndex, int secondIndex)
        {
            SwapVectors(columnVectors, firstIndex, secondIndex);
        }

        private void SwapVectors<TVector>(TVector[] vectorsArray, int firstIndex, int secondIndex) where TVector : BaseVector
        {
            if (Math.Max(firstIndex, secondIndex) >= vectorsArray.Length)
            {
                Debug.LogError("[Data] GameMatrix.SwapVectors: some index is out of range");
                return;
            }
            var temp = vectorsArray[firstIndex];
            vectorsArray[firstIndex] = vectorsArray[secondIndex];
            vectorsArray[secondIndex] = temp;
        }
    }
}