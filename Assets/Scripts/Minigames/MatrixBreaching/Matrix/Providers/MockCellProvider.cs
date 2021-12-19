using System;
using System.Collections.Generic;
using Minigames.MatrixBreaching.Matrix.Interfaces;

namespace Minigames.MatrixBreaching.Matrix.Providers
{
    public class MockCellProvider : ICellProvider
    {
        private Func<int, IEnumerable<ICell>> _newMatrixCellsFunc;
        private Func<ICell> _newCellFunc;

        public MockCellProvider SetMockFunc(Func<int, IEnumerable<ICell>> newMatrixCellsFunc)
        {
            _newMatrixCellsFunc = newMatrixCellsFunc;
            return this;
        }
        public MockCellProvider SetMockFunc(Func<ICell> newCellFunc)
        {
            _newCellFunc = newCellFunc;
            return this;
        }
        public IEnumerable<ICell> GetNewCells(int size)
        {
            return _newMatrixCellsFunc.Invoke(size);
        }

        public ICell GetNewCell()
        {
            return _newCellFunc.Invoke();
        }
    }
}