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
        private Color _dimColor;

        private void Awake()
        {
            
            _dimColor = Background.color;
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            HighlightCell(_combination.HighlightColor);
            GameService.Instance.HighlightAllMatchingMatrixCells(Value, _combination.HighlightColor);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            DimCell(_dimColor);
            GameService.Instance.DimAllMatrixCells(_dimColor);
        }

        public override void HighlightCell(Color color)
        {
            Background.color = color;
        }

        public override void DimCell(Color color)
        {
            Background.color = DimColor;
        }

        public void Initialize(int value, Combination combination)
        {
            Value = value;
            _combination = combination;
        }
    }
}