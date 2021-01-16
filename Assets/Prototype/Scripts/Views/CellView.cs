using UnityEngine;

namespace Prototype.Scripts
{
    public class CellView : MonoBehaviour
    {
        [SerializeField] private RectTransform rowSnap;
        [SerializeField] private RectTransform columnSnap;

        private RectTransform _thisTransform;

    
    
    
        public RectTransform ThisTransform =>
            _thisTransform == null ? _thisTransform = transform as RectTransform : _thisTransform;
        private void Update()
        {
            var rowPos = rowSnap.position;
            var columnPos = columnSnap.position;
            ThisTransform.position = new Vector3(columnPos.x , rowPos.y,
                (rowPos.z + columnPos.z) * 0.5f);
        }
    }
}
