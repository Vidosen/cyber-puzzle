using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Matrix;
using Prototype.Scripts.Matrix;
using Prototype.Scripts.Utils;
using UnityEngine;

namespace Prototype.Scripts.Data
{
    public class MatrixCell : BaseCell
    {
        private Transform holder;
        private GameMatrix _gameMatrix;
        private Sequence _animationSequence;
        public RowVector Row { get; protected set; }
        public ColumnVector Column { get; protected set; }

        protected Dictionary<HighlightType,List<Color>> highlightColors = new Dictionary<HighlightType,List<Color>>();

        public void Initialize(GameMatrix gameMatrix, ColumnVector column, RowVector row)
        {
            _gameMatrix = gameMatrix;
            Row = row;
            Column = column;

            ThisTransform.localScale = Vector3.one;
        }
        public override void Dispose()
        {
            Hide((() => base.Dispose()));
        }

        private void Hide(Action callback)
        {
            _animationSequence?.Kill();
            _animationSequence = DOTween.Sequence()
                .Append(ThisTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutSine))
                .OnComplete((() => callback?.Invoke()));
        }

        private void OnDestroy()
        {
            _animationSequence?.Kill();
        }

        private void Update()
        {
            if (Row == null || Column == null || Row.IsDragging || Column.IsDragging)
                return;
            var rowSlot = _gameMatrix.FindRowSlotByVector(Row);
            var columnSlot = _gameMatrix.FindColumnSlotByVector(Column);
            var rowPos = rowSlot.ThisTransform.anchoredPosition;
            var columnPos = columnSlot.ThisTransform.anchoredPosition;
            ThisTransform.anchoredPosition = new Vector2(columnPos.x + columnSlot.ThisTransform.rect.width / 2, rowPos.y - rowSlot.ThisTransform.rect.height / 2);
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
        public void SetLocalZ(float newZ)
        {
            var newPosition = ThisTransform.localPosition;
            newPosition.z = newZ;
            ThisTransform.localPosition = newPosition;
        }
    }
}