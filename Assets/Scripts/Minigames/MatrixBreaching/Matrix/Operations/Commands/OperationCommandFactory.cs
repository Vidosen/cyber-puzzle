using System;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Commands
{
    
    public class OperationCommandFactory : IFactory<OperationType, RowType, IMatrixCommand>
    {
        private readonly DiContainer _container;

        public OperationCommandFactory(DiContainer container)
        {
            _container = container;
        }
        public IMatrixCommand Create(OperationType operationType, RowType rowType)
        {
            switch (operationType)
            {
                case OperationType.Swap:
                    return GetSwapOperation(rowType);
                case OperationType.Scroll:
                    return GetScrollOperation(rowType);
                default:
                    throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null);
            }
        }

        private IMatrixCommand GetScrollOperation(RowType rowType)
        {
            switch (rowType)
            {
                case RowType.None:
                    throw  new InvalidOperationException();
                case RowType.Horizontal:
                    return _container.Instantiate<HorizontalRowScrollCommand>();
                case RowType.Vertical:
                    return _container.Instantiate<VerticalRowScrollCommand>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(rowType), rowType, null);
            }
        }

        private IMatrixCommand GetSwapOperation(RowType rowType)
        {
            switch (rowType)
            {
                case RowType.None:
                    throw  new InvalidOperationException();
                case RowType.Horizontal:
                    return _container.Instantiate<HorizontalRowsSwapCommand>();
                case RowType.Vertical:
                    return _container.Instantiate<VerticalRowsSwapCommand>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(rowType), rowType, null);
            }
        }
    }
}