using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Views;
using Minigames.MatrixBreaching.Matrix.Views.Cells;
using Minigames.MatrixBreaching.Vulnerabilities.Views;
using UnityEngine;

namespace Minigames.MatrixBreaching.Config
{
    [Serializable]
    struct CellViewTemplateItem
    {
        public CellType Type;
        public BaseCellView View;
    }

    [CreateAssetMenu(fileName = "MatrixBreachingViewConfig", menuName = "Configs/Matrix Breaching/View", order = 0)]
    public class MatrixBreachingViewConfig : ScriptableObject
    {
        [Header("Settings/Guard Matrix")] [SerializeField]
        private Vector2 _genericCellSize;

        [Header("Templates/Guard Matrix")] [SerializeField]
        private GuardMatrixRowView _verticalRowViewTemplate;

        [SerializeField] private GuardMatrixRowView _horizontalRowViewTemplate;

        [SerializeField] private GuardMatrixExchangerView _verticalExchangerViewTemplate;
        [SerializeField] private GuardMatrixExchangerView _horizontalExchangerViewTemplate;
        [SerializeField] private List<CellViewTemplateItem> _cellViewTemplates;

        [Header("Templates/Vulnerability")] [SerializeField]
        private VulnerabilityCellView _vulnerabilityCellViewTemplate;

        [SerializeField] private VulnerabilityView _vulnerabilityViewTemplate;

        [Header("Animation/Guard Matrix")] [SerializeField]
        private Ease _swapFlatMoveEase;

        [SerializeField] private float _swapMoveDuration;
        private Dictionary<CellType, BaseCellView> _cachedCellViewDictionary;

        public Vector2 GenericCellSize => _genericCellSize;
        public GuardMatrixRowView VerticalRowViewTemplate => _verticalRowViewTemplate;
        public GuardMatrixRowView HorizontalRowViewTemplate => _horizontalRowViewTemplate;
        public GuardMatrixExchangerView VerticalExchangerViewTemplate => _verticalExchangerViewTemplate;
        public GuardMatrixExchangerView HorizontalExchangerViewTemplate => _horizontalExchangerViewTemplate;
        public VulnerabilityCellView VulnerabilityCellViewTemplate => _vulnerabilityCellViewTemplate;
        public VulnerabilityView VulnerabilityViewTemplate => _vulnerabilityViewTemplate;
        public Ease SwapFlatMoveEase => _swapFlatMoveEase;
        public float SwapMoveDuration => _swapMoveDuration;

        public BaseCellView GetCellViewTemplate(CellType type)
        {
            if (_cachedCellViewDictionary == null)
                _cachedCellViewDictionary = _cellViewTemplates.ToDictionary(item => item.Type, item => item.View);
            return _cachedCellViewDictionary[type];
        }
    }
}