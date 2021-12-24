using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;
using TMPro;
using UnityEngine;

namespace Minigames.MatrixBreaching.Views
{
    public class ValueCellView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _valueText;
        public RectTransform Transform => _transform == null? GetComponent<RectTransform>() : _transform;
        public ICell Model => _cellModel;
        
        private ValueCell _cellModel;
        private RectTransform _transform;

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
        }
        public void Initialize(ICell cellModel, float scale)
        {
            _cellModel = cellModel as ValueCell;
            if (scale > 0 && _transform != null)
            {
                _transform.sizeDelta = _transform.sizeDelta * scale;
            }

            UpdateView();
        }

        private void UpdateView()
        {
            _valueText.text = ((int)_cellModel.ValueType).ToString();
        }
    }
}