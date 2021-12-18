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
        [SerializeField] private float _moveAnimationDuration = 0.45f;
        private Transform holder;
        private GameMatrix _gameMatrix;
        private Sequence _hideAnimation;
        private Sequence _moveAnimation;
        private Sequence _zPosAnimation;
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
            KillAnimation(_hideAnimation);
            _hideAnimation = DOTween.Sequence()
                .Append(ThisTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutSine))
                .OnComplete((() => callback?.Invoke()));
        }

        private void OnDestroy()
        {
            KillAnimation(_zPosAnimation);
            KillAnimation(_moveAnimation);
            KillAnimation(_hideAnimation);

        }
        private void KillAnimation(Tween tween)
        {
            if (tween.IsActive()) tween.Kill();
        }
        
        public void SnapCell(bool animate = false)
        {
            var rowSlot = _gameMatrix.FindRowSlotByVector(Row);
            var columnSlot = _gameMatrix.FindColumnSlotByVector(Column);
            var rowPos = rowSlot.ThisTransform.anchoredPosition;
            var columnPos = columnSlot.ThisTransform.anchoredPosition;
            var targetPos = new Vector2(columnPos.x + columnSlot.ThisTransform.rect.width / 2,
                rowPos.y - rowSlot.ThisTransform.rect.height / 2);
            if (animate)
            {
                KillAnimation(_moveAnimation);
                _moveAnimation = DOTween.Sequence()
                    .Append(ThisTransform.DOAnchorPos(targetPos, _moveAnimationDuration));
            }
            else
                ThisTransform.anchoredPosition = targetPos;
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
            KillAnimation(_zPosAnimation);
            _zPosAnimation = DOTween.Sequence()
                .Append(ThisTransform.DOLocalMoveZ(newZ, 0.2f));
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