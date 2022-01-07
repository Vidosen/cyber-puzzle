using System;
using System.Collections.Generic;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;

namespace Minigames.MatrixBreaching.Matrix.Providers
{
    public class RandomValueCellProvider : ICellProvider
    {
        private Random _random;
        public void SetRandomSeed(int seed)
        {
            _random = new Random(seed);
        }
        public IEnumerable<ICell> GetNewCells(int horizontalSize, int verticalSize)
        {
            var size = horizontalSize * verticalSize;
            var cellsList = new List<ICell>();
            for (int i = 0; i < size; i++)
            {
                cellsList.Add(GetNewCell());
            }
            return cellsList;
        }

        public ICell GetNewCell()
        {
            return new ValueCell((CellValueType)_random.Next(0, Enum.GetNames(typeof(CellValueType)).Length));
        }
    }
}