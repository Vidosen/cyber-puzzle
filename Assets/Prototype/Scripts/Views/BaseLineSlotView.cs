using UnityEngine;
using UnityEngine.EventSystems;

namespace Prototype.Scripts.Views
{
    public abstract class BaseLineSlotView : MonoBehaviour, IDropHandler
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void OnDrop(PointerEventData eventData)
        {
            var onDragObj = eventData.pointerDrag;
            onDragObj.transform.SetParent(transform);
            Debug.Log("BaseLineSlotView.OnDrop: "+ onDragObj.name);
            
        }
    }
}
