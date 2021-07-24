using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Scripts.Data
{
    public abstract class BaseCell : MonoBehaviour, ICell, IDisposable
    {
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                SetValueText(value.ToString());
                Show();
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
        private Sequence _animationSequence;

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

        
        public void Show(Action onShown = null)
        {
            _animationSequence?.Kill();
            ThisTransform.localScale = Vector3.zero;
            _animationSequence = DOTween.Sequence()
                .Append(ThisTransform.DOScale(Vector3.one, 0.75f).SetEase(Ease.InOutSine))
                .OnComplete((() => onShown?.Invoke()));
        }
        public virtual void Dispose()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            _animationSequence?.Kill();
        }
    }

    public enum HighlightType
    {
        HoverHint,
        CombinationSequence
    }
}