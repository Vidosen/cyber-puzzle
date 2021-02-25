using System;
using UnityEngine;

namespace Prototype.Scripts.Views
{
    public class CombinationCell: MonoBehaviour, IDisposable
    {
        public int Value { get; set; }
        public void Dispose()
        {
            Destroy(gameObject);
        }
        
    }
}