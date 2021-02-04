using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Prototype.Scripts.Views
{
    public abstract class BaseLineSlotView : MonoBehaviour, IDropHandler
    {

        public void OnDrop(PointerEventData eventData)
        {
            var onDragObj = eventData.pointerDrag;
            onDragObj.transform.SetParent(transform);
            Debug.Log("BaseLineSlotView.OnDrop: "+ onDragObj.name);
            
        }
    }
}
