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
        public void Initialize(ICell cellModel)
        {
            _cellModel = cellModel as ValueCell;
            UpdateView();
        }
        public void Rescale(float scaleFactor)
        {
            if (scaleFactor > 0 && Transform != null)
            {
                Transform.sizeDelta = Transform.sizeDelta * scaleFactor;   
            }
        }

        private void UpdateView()
        {
            _valueText.text = ((int)_cellModel.ValueType).ToString();
        }
    }
}