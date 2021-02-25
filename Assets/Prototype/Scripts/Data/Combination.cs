using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Prototype.Scripts.Views
{
    public class Combination : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDisposable
    {
        private DiContainer Container;

        [Inject]
        private void Construct(DiContainer container)
        {
            Container = container;
        }
        
        public void Dispose()
        {
            Destroy(gameObject);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }
    }
}