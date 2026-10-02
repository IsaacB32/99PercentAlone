using System;
using System.Collections;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace ITween.Animator
{
    public class GroupAnimator : TweenAnimator
    {
        [SerializeField] private TweenAnimator[] _animatorGroup;

        [SerializeField, Tooltip("Tweens play one after the other")]
        private bool _animateInSequence;

        [SerializeField] private bool _reverseOrderOnHide;
        [HorizontalLine] [SerializeField] private float _timeBetween = 0f;
        [SerializeField] private bool _ignoreTimeScale = false;

        private ProtectedCoroutine _activeSequence;
        private TweenAnimator[] _orderedList;

        protected override VisibilityTween InitializeTween()
        {
            if (_animatorGroup.Length == 0) throw new Exception("Trying to initialize GroupAnimator with no animators");
            ForceInitializeGroup(_animatorGroup);
            return null;
        }

        public override VisibilityTween SetVisible(bool visible, bool animate = true, Action onComplete = null)
        {
            _activeSequence.StopIfRunning();
            if (!animate)
            {
                foreach (TweenAnimator animator in _animatorGroup) animator.SetVisible(visible, false);
                onComplete?.Invoke();
                return null;
            }

            TweenAnimator[] orderedList = OrderAnimators(visible);
            _activeSequence = Delay.StartProtectedCoroutine(RunGroup(orderedList, visible, onComplete));
            return null;
        }

        private TweenAnimator[] OrderAnimators(bool visible)
        {
            TweenAnimator[] orderedList = _animatorGroup;
            if (_reverseOrderOnHide && !visible) orderedList = orderedList.Reverse().ToArray();
            return orderedList;
        }

        private IEnumerator RunGroup(TweenAnimator[] animators, bool visible, Action onComplete)
        {
            if (animators.Length == 0)
            {
                onComplete?.Invoke();
                yield break;
            }

            int remaining = animators.Length;
            for (int i = 0; i < animators.Length; i++)
            {
                TweenAnimator animator = animators[i];
                
                if (animator is GroupAnimator groupAnimator)
                {
                    TweenAnimator[] orderedList = groupAnimator.OrderAnimators(visible);
                    yield return groupAnimator.RunGroup(orderedList, visible, () =>
                    {
                        remaining--;
                        if (remaining == 0) onComplete?.Invoke();
                    });
                }
                else
                {
                    bool childDone = false;
                    animator.SetVisible(visible, onComplete: () =>
                    {
                        childDone = true;
                        remaining--;
                        if (remaining == 0) onComplete?.Invoke();
                    });
                    
                    if (_animateInSequence) yield return new WaitUntil(() => childDone);
                }
                
                if (i < animators.Length - 1)
                {
                    yield return _ignoreTimeScale ? 
                        new WaitForSecondsRealtime(_timeBetween) 
                        : new WaitForSeconds(_timeBetween);
                }
            }
        }
    }
}
