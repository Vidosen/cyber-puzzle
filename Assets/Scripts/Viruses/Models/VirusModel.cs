using System;
using UniRx;
using UnityEngine;

namespace Viruses.Models
{
    public class VirusModel
    {
        private ReactiveProperty<float> _currentDurability = new ReactiveProperty<float>();
        
        public string Id { get; protected set; }
        //Breaching
        public float BreachDamage { get; protected set; }
        
        //Durability
        public IReadOnlyReactiveProperty<float> CurrentDurability => _currentDurability;
        public float MaxDurability { get; protected set; }
        public float RestoreDurabilitySpeed { get; protected set; }

        public IObservable<Unit> OutOfDurabilityObservable =>
            _currentDurability.Where(durability=>durability <= float.Epsilon).AsUnitObservable();

        public VirusModel Initialize(string id, float breachDamage, float maxDurability, float resetoreDurabilitySpeed)
        {
            Id = id;
            BreachDamage = breachDamage;
            _currentDurability.Value = MaxDurability = maxDurability;
            RestoreDurabilitySpeed = resetoreDurabilitySpeed;
            return this;
        }

        public void DealDamage(float damageToVirus)
        {
            ChangeDurability(_currentDurability.Value - damageToVirus);
        }

        private void ChangeDurability(float newDurabilityValue)
        {
            _currentDurability.Value = Mathf.Clamp(newDurabilityValue, 0, MaxDurability);
        }
    }
}