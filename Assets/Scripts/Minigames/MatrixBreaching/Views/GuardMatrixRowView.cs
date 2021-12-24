using UnityEngine;

namespace Minigames.MatrixBreaching.Views
{
    public class GuardMatrixRowView : MonoBehaviour
    {
        [SerializeField] private RowType _rowType;
        private RectTransform _transform;
        
        public RowType RowType => _rowType;
        public int Index { get; private set; }
        public RectTransform Transform => _transform;

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
        }

        public void Initialize(int index, float scale)
        {
            Index = index;
            if (scale > 0 && _transform != null)
            {
                _transform.sizeDelta = _transform.sizeDelta * scale;   
            }
        }
    }
}