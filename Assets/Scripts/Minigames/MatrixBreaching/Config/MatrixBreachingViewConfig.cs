using DG.Tweening;
using Minigames.MatrixBreaching.Matrix.Views;
using Minigames.MatrixBreaching.Vulnerabilities.Views;
using UnityEngine;

namespace Minigames.MatrixBreaching.Config
{
    [CreateAssetMenu(fileName = "MatrixBreachingViewConfig", menuName = "Configs/Matrix Breaching View", order = 0)]
    public class MatrixBreachingViewConfig : ScriptableObject
    {
        [Header("Templates/Guard Matrix")]
        [SerializeField] private GuardMatrixRowView _verticalRowViewTemplate;
        [SerializeField] private GuardMatrixRowView _horizontalRowViewTemplate;
        
        [SerializeField] private GuardMatrixExchangerView _verticalExchangerViewTemplate;
        [SerializeField] private GuardMatrixExchangerView _horizontalExchangerViewTemplate;
        [SerializeField] private ValueCellView _valueCellViewTemplate;

        [Header("Templates/Vulnerability")]
        [SerializeField] private VulnerabilityCellView _vulnerabilityCellViewTemplate;
        [SerializeField] private VulnerabilityView _vulnerabilityViewTemplate;
        
        [Header("Animation/Guard Matrix")]
        [SerializeField] private Ease _swapFlatMoveEase;
        [SerializeField] private float _swapMoveDuration;

        public GuardMatrixRowView VerticalRowViewTemplate => _verticalRowViewTemplate;
        public GuardMatrixRowView HorizontalRowViewTemplate => _horizontalRowViewTemplate;
        public GuardMatrixExchangerView VerticalExchangerViewTemplate => _verticalExchangerViewTemplate;
        public GuardMatrixExchangerView HorizontalExchangerViewTemplate => _horizontalExchangerViewTemplate;
        public ValueCellView ValueCellViewTemplate => _valueCellViewTemplate;
        public VulnerabilityCellView VulnerabilityCellViewTemplate => _vulnerabilityCellViewTemplate;
        public VulnerabilityView VulnerabilityViewTemplate => _vulnerabilityViewTemplate;
        public Ease SwapFlatMoveEase => _swapFlatMoveEase;
        public float SwapMoveDuration => _swapMoveDuration;
    }
}