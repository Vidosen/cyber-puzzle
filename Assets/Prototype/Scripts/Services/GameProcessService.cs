using Prototype.Scripts.Data;

namespace Prototype.Scripts.Services
{
    public class GameProcessService : IGameService
    {
        private GameMatrix _matrix;
        public void Initialize()
        {
            _matrix = new GameMatrix(4,4);
        }
    }
}