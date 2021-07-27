using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Matrix
{
    public class MatrixCell : BaseCell, IBeginDragHandler, IEndDragHandler, IDragHandler
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

        private void Hide(Action callback)
        {
            if (_animationSequence.IsActive())
                _animationSequence.Kill();
            _animationSequence = DOTween.Sequence()
                .Append(ThisTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutSine))
                .OnComplete((() => callback?.Invoke()));
        }

        private void OnDestroy()
        {
            if (_animationSequence.IsActive())
                _animationSequence.Kill();
            
            if (_zPosAnimation.IsActive())
                _zPosAnimation.Kill();

        }

        private void Update()
        {
            if (Row == null || Column == null || Row.IsDragging || Column.IsDragging)
                return;
            //SnapCell();
        }

        public void SnapCell()
        {
            var rowSlot = _gameMatrix.FindRowSlotByVector(Row);
            var columnSlot = _gameMatrix.FindColumnSlotByVector(Column);
            var rowPos = rowSlot.ThisTransform.anchoredPosition;
            var columnPos = columnSlot.ThisTransform.anchoredPosition;
            ThisTransform.anchoredPosition = new Vector2(columnPos.x + columnSlot.ThisTransform.rect.width / 2,
                rowPos.y - rowSlot.ThisTransform.rect.height / 2);
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
        private Tween _zPosAnimation;
        public void SetLocalZ(float newZ)
        {
            if (_zPosAnimation.IsActive())
            {
                _zPosAnimation.Kill();
            }
            _zPosAnimation = ThisTransform.DOLocalMoveZ(newZ, 0.2f).Play();
        }

        private Vector2 _startPos;
        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPos = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var delta = eventData.position - _startPos;
            
        }

        public void OnDrag(PointerEventData eventData)
        {
            //throw new NotImplementedException();
        }
    }
}