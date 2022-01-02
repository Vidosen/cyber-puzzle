using Minigames.MatrixBreaching.Matrix.Data;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Interfaces
{
    public interface IMatrixCommand
    {
        void Execute();
        void Cancel();

        public class Factory : PlaceholderFactory<OperationType, RowType, IMatrixCommand>
        {
            
        }
    }
}