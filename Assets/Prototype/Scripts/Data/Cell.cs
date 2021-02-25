using System;
using TMPro;
using UnityEngine;

namespace Prototype.Scripts.Data
{
    public class Cell : MonoBehaviour
    {
        public RowVector Row { get; protected set; }
        public ColumnVector Column { get; protected set; }
        
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
        
        private RectTransform _rowSnap;
        private RectTransform _columnSnap;
        
        [SerializeField]
        private TextMeshProUGUI cellValueText;

        private int _value;
        private RectTransform _thisTransform;
        public void Initialize(ColumnVector column, RowVector row)
        {
            Row = row;
            _rowSnap = row.ThisTransform;
            
            Column = column;
            _columnSnap = column.ThisTransform;
            
            ThisTransform.localScale = Vector3.one;
        }

        public void SetValueText(string value)
        {
            cellValueText.text = value;
        }
        private void Update()
        {
            if (_rowSnap == null || _columnSnap == null)
                return;
            
            var rowPos = _rowSnap.position;
            var columnPos = _columnSnap.position;
            ThisTransform.position = new Vector3(columnPos.x , rowPos.y,
                (rowPos.z + columnPos.z) * 0.5f);
        }
        public void SetSnapViews(RectTransform _rowSnap, RectTransform _columnSnap)
        {
            this._rowSnap = _rowSnap;
            this._columnSnap = _columnSnap;
        }
    }
}