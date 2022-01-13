using System;
using System.Collections.Generic;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models.Cells;
using Utils;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Providers
{
    public class RandomCellProvider : ICellProvider
    {
        private readonly DiContainer _container;
        private Random _random;

        public RandomCellProvider(DiContainer container)
        {
            _container = container;
        }
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
            var cellType = CoreExtensions.GetRandomEnum<CellType>(_random);
            switch (cellType)
            {
                case CellType.Value:
                {
                    var cellValue = CoreExtensions.GetRandomEnum<CellValueType>(_random);
                    return _container.Instantiate<ValueCell>(new object[] { cellValue });
                }
                case CellType.Glitch:
                {
                    var cellValue = CoreExtensions.GetRandomEnum<CellValueType>(_random);
                    return _container.Instantiate<GlitchCell>(new object[] { cellValue });
                }
                case CellType.Shuffle:
                    return _container.Instantiate<ShuffleCell>();
                case CellType.Lock:
                    return _container.Instantiate<LockCell>();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}