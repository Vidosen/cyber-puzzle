using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Matrix
{
    public abstract class BaseVector : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IDisposable
    {
        
        public int Size => cells?.Length ?? 0;
        public MatrixCell[] Cells => cells;
        public RectTransform ThisTransform =>
            _thisTransform == null ? _thisTransform = transform as RectTransform : _thisTransform;
        public bool IsDragging => _isDragging;
        

        public bool IsInitialized => cells != null;
        public abstract Vector2 SnapDirection { get; protected set; }

        private MatrixCell[] cells;
        [SerializeField] private float _HighlightZOffset = 30;
        [SerializeField] private TextMeshProUGUI LineIndexText;
        [SerializeField, Range(0, 1)] private float DragAlpha = 1f;
        private RectTransform _thisTransform;
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        private bool _isDragging;
        private float _deafultZ;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _canvasGroup = GetComponentInParent<CanvasGroup>();
            _deafultZ = ThisTransform.localPosition.z;
        }

        private void OnDestroy()
        {
            if(cells != null)
                foreach (var cell in cells)
                    if (cell != null)
                        Destroy(cell.gameObject);
        }

        public MatrixCell this[int index]
        {
            get
            {
                if (index >= cells.Length)
                {
                    Debug.LogError("[Data] BaseVector.this[int index]: index is out of range");
                    return default;
                }
                return cells[index];
            }
            set
            {
                if (index >= cells.Length)
                    {
                        Debug.LogError($"[Data] {GetType().Name}.this[int index]: index is out of range");
                        return;
                    }
                cells[index] = value;
            }
        }
        
        public int IndexOfCell(MatrixCell matrixCell)
        {
            for (int i = 0; i < Size; i++)
            {
                if (matrixCell.Equals(cells[i]))
                {
                    return i;
                }
            }
            return -1;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            ThisTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            foreach (var cell in cells)
            {
                cell.UnpinVector();
                cell.SetLocalZ(_deafultZ);
            }
            
            _isDragging = false;
            _canvasGroup.blocksRaycasts = !_isDragging;
            _canvasGroup.alpha = 1f;
            
            SetSortingOrder(10);
            SetLocalZ(_deafultZ);
            ThisTransform.anchoredPosition = Vector3.zero;
            Array.ForEach(Cells, cell => cell.SnapCell());
            Debug.Log($"{GetType().Name}.OnEndDrag");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
            _canvasGroup.blocksRaycasts = !_isDragging;
            _canvasGroup.alpha = DragAlpha;

            SetSortingOrder(100);
            SetLocalZ(_deafultZ - _HighlightZOffset);
            foreach (var cell in cells)
            {
                cell.PinVector(ThisTransform);
                cell.SetLocalZ(_deafultZ - _HighlightZOffset);
            }
        }

        public BaseVector Initialize(int cellsCount, int index)
        {
            cells = new MatrixCell[cellsCount];
            SetLineIndex(index.ToString());
            ThisTransform.localScale = Vector3.one;
            ThisTransform.anchoredPosition = Vector3.zero;
            return this;
        }
        

        public void SetLineIndex(string lineIndex)
        {
            LineIndexText.text = lineIndex;
        }

        public void Dispose()
        {
            if (_animationTweener.IsActive())
            {
                _animationTweener.Kill();
            }
            if (cells != null)
                Array.ForEach(cells, c => c.Dispose());
            Destroy(gameObject);
        }

        private Tween _animationTweener;
        private void SetLocalZ(float newZ)
        {
            if (_animationTweener.IsActive())
            {
                _animationTweener.Kill();
            }
            _animationTweener = ThisTransform.DOLocalMoveZ(newZ, 0.2f).Play();
        }

        private void SetSortingOrder(int order) => _canvas.sortingOrder = order;
    }
}