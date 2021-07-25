using System.Collections.Generic;
using Matrix;
using Prototype.Scripts.Data;
using Services;
using Signals;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Prototype.Scripts.Combinations
{
    public class CombinationCell : BaseCell, IPointerEnterHandler, IPointerExitHandler
    {
        protected Dictionary<HighlightType, Color> highlightColors = new Dictionary<HighlightType, Color>();

        public readonly Subject<CombinationCell> CellHoverEnter = new Subject<CombinationCell>();
        public readonly Subject<CombinationCell> CellHoverExit = new Subject<CombinationCell>();

        public void OnPointerEnter(PointerEventData eventData)
        {
            CellHoverEnter.OnNext(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CellHoverExit.OnNext(this);
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

        public void Initialize(int value)
        {
            Value = value;
        }

        public override void Dispose()
        {
            CellHoverEnter.OnCompleted();
            CellHoverExit.OnCompleted();
            CellHoverEnter.Dispose();
            CellHoverExit.Dispose();
            base.Dispose();
        }
    }
}