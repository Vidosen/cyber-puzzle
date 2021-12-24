using System;
using UnityEngine;

namespace Minigames.MatrixBreaching.Views
{
    public class GuardMatrixExchangerView : MonoBehaviour
    {
        [SerializeField] private RowType _rowType;
        private RectTransform _transform;
        private GuardMatrixRowView _rootRow;

        public RowType RowType => _rowType;
        public RectTransform Transform => _transform;

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
        }

        public void Initialize(GuardMatrixRowView rowView, float scale)
        {
            if (scale > 0 && _transform != null)
            {
                _transform.sizeDelta = _transform.sizeDelta * scale;   
            }
            ChangeRootRow(rowView);
        }

        public void ChangeRootRow(GuardMatrixRowView rowView)
        {
            _rootRow = rowView;
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}