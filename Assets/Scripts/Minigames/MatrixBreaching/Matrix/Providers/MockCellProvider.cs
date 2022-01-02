using System;
using System.Collections.Generic;
using Minigames.MatrixBreaching.Matrix.Interfaces;

namespace Minigames.MatrixBreaching.Matrix.Providers
{
    public class MockCellProvider : ICellProvider
    {
        private Func<int ,int, IEnumerable<ICell>> _newMatrixCellsFunc;
        private Func<ICell> _newCellFunc;

        public MockCellProvider SetMockFunc(Func<int, int, IEnumerable<ICell>> newMatrixCellsFunc)
        {
            _newMatrixCellsFunc = newMatrixCellsFunc;
            return this;
        }
        public MockCellProvider SetMockFunc(Func<ICell> newCellFunc)
        {
            _newCellFunc = newCellFunc;
            return this;
        }
        public IEnumerable<ICell> GetNewCells(int horizontalSize, int verticalSize)
        {
            return _newMatrixCellsFunc.Invoke(horizontalSize, verticalSize);
        }

        public ICell GetNewCell()
        {
            return _newCellFunc.Invoke();
        }
    }
}