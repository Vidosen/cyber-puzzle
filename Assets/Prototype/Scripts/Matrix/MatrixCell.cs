using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Scripts.Data
{
    public class MatrixCell : BaseCell
    {
        public RowVector Row { get; protected set; }
        public ColumnVector Column { get; protected set; }

        protected List<Color> highlightColors = new List<Color>();

        public void Initialize(ColumnVector column, RowVector row)
        {
            Row = row;
            Column = column;

            ThisTransform.localScale = Vector3.one;
        }
        
        private void Update()
        {
            if (Row == null || Column == null || Row.IsDragging || Column.IsDragging)
                return;
            
            var rowPos = Row.ThisTransform.position;
            var columnPos = Column.ThisTransform.position;
            ThisTransform.position = new Vector3(columnPos.x , rowPos.y,
                (rowPos.z + columnPos.z) * 0.5f);
        }

        public override void HighlightCell(Color color)
        {
            highlightColors.Add(color);
            float finalH = 0, finalS = 0, finalV = 0;
            foreach (var _color in highlightColors)
            {
                Color.RGBToHSV(_color, out float h, out float s, out float v);
                finalH += h;
                finalS += s;
                finalV += v;
            }

            var size = highlightColors.Count;
            Background.color = Color.HSVToRGB(finalH / size, finalS / size, finalV / size);
        }

        public override void DimCell(Color color)
        {
            highlightColors.Clear();
            Background.color = DimColor;
        }

        private Transform holder;
        public void PinVector(Transform vecTransform)
        {
            if (Row.transform != vecTransform && Column.transform != vecTransform)
                return;
            holder = ThisTransform.parent;
            ThisTransform.SetParent(vecTransform);
        }
        public void UnpinVector()
        {
            ThisTransform.SetParent(holder);
        }
    }
}