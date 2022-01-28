using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Loading.View
{
    public class LoadingView : MonoBehaviour
    {
        [SerializeField] private float _loadAnimationInverval;
        [SerializeField] private TextMeshProUGUI _loadingAnimationDotsText;
        private Tween _animationTween;

        void Start()
        {
            _animationTween = _loadingAnimationDotsText.DOText("...", _loadAnimationInverval)
                .From("")
                .SetLoops(-1, LoopType.Restart);
        }

        private void OnDestroy()
        {
            _animationTween?.Kill();
        }
    }
}
