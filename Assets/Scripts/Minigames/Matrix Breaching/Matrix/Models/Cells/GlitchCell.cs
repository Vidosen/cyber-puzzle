using System;
using Minigames.MatrixBreaching.Matrix.Data;
using UniRx;
using Zenject;

namespace Minigames.MatrixBreaching.Matrix.Models.Cells
{
    public sealed class GlitchCell : ValueCell
    {
        public override CellType CellType => CellType.Glitch;
        private Subject<CellValueType> _valueChangedSubject = new Subject<CellValueType>();
        public IObservable<CellValueType> ValueChanged => _valueChangedSubject;
        
        public GlitchCell(CellValueType value, SignalBus signalBus) : base(value, signalBus)
        {
        }

        public void ChangeValue(CellValueType newValue)
        {
            _valueChangedSubject.OnNext(newValue);
            Value = newValue;
        }
    }
}