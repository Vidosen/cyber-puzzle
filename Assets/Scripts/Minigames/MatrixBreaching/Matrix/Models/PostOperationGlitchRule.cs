using System;
using System.Linq;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Models.Cells;
using Minigames.MatrixBreaching.Matrix.Signals;
using Utils;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Models
{
    public class PostOperationGlitchRule : IInitializable
    {
        private readonly SignalBus _signalBus;
        private Random _random;

        public PostOperationGlitchRule(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }
        public void Initialize()
        {
            _signalBus.Subscribe<MatrixOperationsSignals.OperationApplied>(ApplyGlitchForInvolvedCells);
        }
        public void SetRandomSeed(int seed)
        {
            _random = new Random(seed);
        }
        private void ApplyGlitchForInvolvedCells(MatrixOperationsSignals.OperationApplied signal)
        {
            var glitchCells = signal.InvolvedCells.Where(cell => cell.CellType == CellType.Glitch)
                .Select(cell => (GlitchCell)cell);
            
            foreach (var glitchCell in glitchCells)
            {
                var newValue = CoreExtensions.GetRandomEnum<CellValueType>(_random);
                glitchCell.ChangeValue(newValue);
            }
        }
    }
}