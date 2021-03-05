using System;
using System.Collections.Generic;
using Prototype.Scripts.Data;
using Prototype.Scripts.Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Prototype.Scripts.Views
{
    public class CombinationCell : BaseCell, IDisposable, IPointerEnterHandler, IPointerExitHandler
    {
        private Combination _combination;
        protected Dictionary<HighlightType, Color> highlightColors = new Dictionary<HighlightType, Color>();
        public void Dispose()
        {
            Destroy(gameObject);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            HighlightCell(_combination.HighlightColor, HighlightType.HoverHint);
            GameService.Instance.HighlightAllMatchingMatrixCells(Value, _combination.HighlightColor, HighlightType.HoverHint);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            DimCell(HighlightType.HoverHint);
            GameService.Instance.DimAllMatrixCells(HighlightType.HoverHint);
        }

        public override void HighlightCell(Color color, HighlightType type)
        {
            if (!highlightColors.ContainsKey(type))
                highlightColors.Add(type, color);
            Background.color = color;
        }

        public override void DimCell(HighlightType type)
        {
            if (highlightColors.ContainsKey(type))
                highlightColors.Remove(type);
            
            Background.color = DimColor;
        }

        public void Initialize(int value, Combination combination)
        {
            Value = value;
            _combination = combination;
        }
    }
}