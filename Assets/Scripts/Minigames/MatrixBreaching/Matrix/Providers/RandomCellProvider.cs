﻿using System;
using System.Collections.Generic;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;
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
                cellsList.Add(GetNewCell(cellsList));
            }
            return cellsList;
        }

        public ICell GetNewCell(List<ICell> existingCells)
        {
            var lockWeight = existingCells.GetCells<LockCell>().Count < 2? 15 : 0;
            var shuffleWeight = existingCells.GetCells<ShuffleCell>().Count < 2? 10 : 0;
            
            var cellType = _random.GetRandomItem(new Dictionary<CellType, int>()
            {
                { CellType.Glitch , 10 },
                { CellType.Lock , lockWeight },
                { CellType.Shuffle , shuffleWeight },
                { CellType.Value, 65 }
            });
            
            var cellValue = _random.GetRandomEnum<CellValueType>();
            switch (cellType)
            {
                case CellType.Value:
                    return _container.Instantiate<ValueCell>(new object[] { cellValue });
                case CellType.Glitch:
                    return _container.Instantiate<GlitchCell>(new object[] { cellValue });
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