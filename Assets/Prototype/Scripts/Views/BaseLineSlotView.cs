using UnityEngine;
using UnityEngine.EventSystems;

namespace Prototype.Scripts
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
            throw new System.NotImplementedException();
        }
    }
}
