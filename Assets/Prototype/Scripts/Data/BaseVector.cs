using System;
using System.Linq;
using ModestTree;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Prototype.Scripts.Data
{
    public abstract class BaseVector : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler {

        public int Size => cells.Length;
        public RectTransform ThisTransform =>
            _thisTransform == null ? _thisTransform = transform as RectTransform : _thisTransform;
        public bool VectorInitialized => cells != null;
        public abstract Vector2 SnapDirection { get; protected set; }

        private Cell[] cells;
        
        [SerializeField]
        private TextMeshProUGUI LineIndexText;
        private RectTransform _thisTransform;
        private Canvas _canvas;
        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
        }

        private void OnDestroy()
        {
            if(cells != null)
                foreach (var cell in cells)
                    if (cell != null)
                        Destroy(cell.gameObject);
        }

        public Cell this[int index]
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
        
        public int IndexOfCell(Cell cell)
        {
            for (int i = 0; i < Size; i++)
            {
                if (cell.Equals(cells[i]))
                {
                    return i;
                }
            }
            return -1;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            ThisTransform.anchoredPosition += SnapDirection * Vector2.Dot(SnapDirection, eventData.delta / _canvas.scaleFactor);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ThisTransform.anchoredPosition = Vector3.zero;
            Debug.Log("BaseLineView.OnEndDrag");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.LogWarning("BaseLineView.OnBeginDrag not implemented");
        }

        public BaseVector Initialize(int cellsCount, int index)
        {
            cells = new Cell[cellsCount];
            SetLineIndex(index.ToString());
            ThisTransform.localScale = Vector3.one;
            ThisTransform.anchoredPosition = Vector3.zero;
            return this;
        }

        public void SetLineIndex(string lineIndex)
        {
            LineIndexText.text = lineIndex;
        }
    }
}