﻿using System;
using System.Collections.Generic;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models.Cells;
using Utils;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Providers
{
    public class RandomValueCellProvider : ICellProvider
    {
        private readonly DiContainer _container;
        private Random _random;

        public RandomValueCellProvider(DiContainer container)
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
            var enumIndex = _random.Next(0, CoreExtensions.GetEnumSize<CellValueType>());
            var cellValue =
                (CellValueType) Enum.Parse(typeof(CellValueType), Enum.GetNames(typeof(CellValueType))[enumIndex]);
            return _container.Instantiate<ValueCell>(new object[] { cellValue });
        }
    }
}