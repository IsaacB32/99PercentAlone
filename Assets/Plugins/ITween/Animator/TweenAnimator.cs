using UnityEngine;

namespace ITween.Animator
{
    /// <summary>
    /// Wrapper for Tweens to show Editor playback buttons
    /// </summary>
    public abstract class TweenAnimator : MonoBehaviour
    {
        public Tween ActiveTween { get; private set; }
        
        protected virtual void Awake()
        {
            ActiveTween = InitializeTween();
        }
        
        protected abstract Tween InitializeTween();
    }
}
