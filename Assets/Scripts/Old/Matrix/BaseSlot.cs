using System;
using Data;
using DG.Tweening;
using Signals;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Matrix
{
    public abstract class BaseSlot<TVector> : MonoBehaviour, IDisposable where TVector : BaseVector
    {

        private RectTransform _thisTransform;
        private TVector _vector;
        private Sequence _moveAnimation;
        [SerializeField] private float _moveAnimationDuration = 0.1f;

        public TVector Vector
        {
            get => _vector;
            set
            {
                _vector = value;
                _vector.ThisTransform.localScale = Vector3.one;
            }
        }
        public void SnapVector(bool animate = false, BaseSlot<TVector> targetVector = null)
        {
            var targetPos = targetVector != null? targetVector.ThisTransform.anchoredPosition : ThisTransform.anchoredPosition;
            if (animate)
            {
                KillAnimation(_moveAnimation);
                _moveAnimation = DOTween.Sequence()
                    .Append(Vector.ThisTransform.DOAnchorPos(targetPos, _moveAnimationDuration));
            }
            else
                Vector.ThisTransform.anchoredPosition = targetPos;
        }

        private void KillAnimation(Tween tween)
        {
            if (tween.IsActive()) tween.Kill();
        }

        public RectTransform ThisTransform =>
            _thisTransform == null ? _thisTransform = transform as RectTransform : _thisTransform;
        
        /*public void OnDrop(PointerEventData eventData)
        {
            var onDragObj = eventData.pointerDrag;
            var oneVector = onDragObj.GetComponent<BaseVector>();
            if (oneVector is null)
                return;
            else if (oneVector.GetType() == Vector.GetType() && oneVector != Vector)
            {
                Debug.Log($"{GetType().Name}.OnDrop: " + onDragObj.name);
                MessageBroker.Default.Publish(new MatrixSignals.VectorSwapRequest() {ActiveVector =  oneVector, PassiveVector =  _vector });
            }

        }*/

        public BaseSlot<TVector> Initialize(TVector vector)
        {
            Vector = vector;
            return this;
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            KillAnimation(_moveAnimation);
        }
    }
}
