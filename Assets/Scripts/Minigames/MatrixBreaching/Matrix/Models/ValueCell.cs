using Minigames.MatrixBreaching.Matrix.Data;
using UniRx;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Models
{
    public class ValueCell : BaseCell
    {
        private ReactiveProperty<bool> _isLit = new ReactiveProperty<bool>();
        public CellValueType Value { get; }
        public IReadOnlyReactiveProperty<bool> IsLit => _isLit;

        public ValueCell(CellValueType value, SignalBus signalBus) : base(signalBus)
        {
            Value = value;
        }
    }
}