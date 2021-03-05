using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Scripts.Data
{
    public abstract class BaseCell : MonoBehaviour
    {
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                SetValueText(value.ToString());
            }
        }
        public RectTransform ThisTransform =>
            _thisTransform == null ? _thisTransform = transform as RectTransform : _thisTransform;
        
        
        public Image Background;
        protected Color DimColor;
        
        [SerializeField]
        private TextMeshProUGUI cellValueText;
        private int _value;
        private RectTransform _thisTransform;
        
        protected virtual void Awake()
        {
            DimColor = Background.color;
        }
        public void SetValueText(string value)
        {
            cellValueText.text = value;
        }

        public abstract void HighlightCell(Color color, HighlightType type);
        public abstract void DimCell(HighlightType type);
    }

    public enum HighlightType
    {
        HoverHint,
        CombinationSequence
    }
}