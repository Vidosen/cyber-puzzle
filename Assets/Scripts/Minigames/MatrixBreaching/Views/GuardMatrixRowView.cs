using Minigames.MatrixBreaching.Matrix.Data;
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

        public void Initialize(int index)
        {
            Index = index;
        }

        public void Rescale(float scaleFactor)
        {
            if (scaleFactor > 0 && Transform != null)
            {
                Transform.sizeDelta = Transform.sizeDelta * scaleFactor;   
            }
        }
    }
}