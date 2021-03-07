﻿using System;
using Prototype.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Prototype.Scripts.Matrix
{
    public abstract class BaseVector : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IDisposable {

        public int Size => cells?.Length ?? 0;
        public MatrixCell[] Cells => cells;
        public RectTransform ThisTransform =>
            _thisTransform == null ? _thisTransform = transform as RectTransform : _thisTransform;
        public bool IsDragging => _isDragging;

        public bool IsInitialized => cells != null;
        public abstract Vector2 SnapDirection { get; protected set; }

        private MatrixCell[] cells;
        
        [SerializeField] private TextMeshProUGUI LineIndexText;
        [SerializeField, Range(0, 1)] private float DragAlpha = 1f;
        private RectTransform _thisTransform;
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        private bool _isDragging;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _canvasGroup = GetComponentInParent<CanvasGroup>();
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
            //ThisTransform.anchoredPosition += SnapDirection * Vector2.Dot(SnapDirection, eventData.delta / _canvas.scaleFactor);
            ThisTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            foreach (var cell in cells)
                cell.UnpinVector();
            
            _isDragging = false;
            _canvasGroup.blocksRaycasts = !_isDragging;
            _canvasGroup.alpha = 1f;
            
            _canvas.sortingOrder = 10;
            
            ThisTransform.anchoredPosition = Vector3.zero;
            Debug.Log($"{GetType().Name}.OnEndDrag");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
            _canvasGroup.blocksRaycasts = !_isDragging;
            _canvasGroup.alpha = DragAlpha;

            _canvas.sortingOrder = 100;
            
            foreach (var cell in cells)
                cell.PinVector(ThisTransform);
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
            if (cells != null)
                Array.ForEach(cells, c => c.Dispose());
            Destroy(gameObject);
        }
    }
}