using System;
using System.Collections.Generic;
using Prototype.Scripts.Views;
using UnityEngine;

namespace Prototype.Scripts.Services
{
    public class ViewService : MonoBehaviour, IGameService
    {
        private List<RowLineView> rowViewsPool = new List<RowLineView>();
        private List<RowLineSlotView> rowSlotViewsPool = new List<RowLineSlotView>();
        private List<ColumnLineView> columnViewsPool = new List<ColumnLineView>();
        private List<ColumnLineSlotView> columnSlotViewsPool = new List<ColumnLineSlotView>();
        private List<CellView> cellViewsPool = new List<CellView>();
        public void Initialize()
        {
            rowViewsPool.AddRange(FindObjectsOfType<RowLineView>());
            rowSlotViewsPool.AddRange(FindObjectsOfType<RowLineSlotView>());
            columnViewsPool.AddRange(FindObjectsOfType<ColumnLineView>());
            columnSlotViewsPool.AddRange(FindObjectsOfType<ColumnLineSlotView>());
            cellViewsPool.AddRange(FindObjectsOfType<CellView>());
        }

        private void Awake()
        {
            Debug.Log("I'm OK");
        }
    }
}