using System;
using System.Collections.Generic;
using System.Linq;
using Prototype.Scripts.Utils;
using UnityEngine;

namespace Prototype.Scripts.Data
{
    public class MatrixCell : BaseCell
    {
        public RowVector Row { get; protected set; }
        public ColumnVector Column { get; protected set; }

        protected Dictionary<HighlightType,List<Color>> highlightColors = new Dictionary<HighlightType,List<Color>>();

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

        public override void HighlightCell(Color color, HighlightType type)
        {
            if (!highlightColors.ContainsKey(type))
                highlightColors.Add(type, new List<Color>());
            
            highlightColors[type].Add(color);
            CalculateColor();
        }

        public override void DimCell(HighlightType type)
        {
            if (highlightColors.ContainsKey(type))
                highlightColors.Remove(type);
            CalculateColor();
        }

        private void CalculateColor()
        {
            if(highlightColors.ContainsKey(HighlightType.HoverHint))
            {
                Background.color = highlightColors[HighlightType.HoverHint].FirstOrDefault();
                return;
            }
            else if (highlightColors.ContainsKey(HighlightType.CombinationSequence))
            {
                Background.color = highlightColors[HighlightType.CombinationSequence].AverageColor();
                return;
            }
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