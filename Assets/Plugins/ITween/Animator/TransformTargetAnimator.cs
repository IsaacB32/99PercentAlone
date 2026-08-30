using UnityEngine;

namespace ITween.Animator
{
    /// <summary>
    /// Moves transform to target
    /// </summary>
    public class TransformTargetAnimator : TweenAnimator
    {
        [SerializeField] protected Transform _target;
        [SerializeField] protected TweenSettings _settings;
        
        protected override Tween InitializeTween()
        {
            return transform.IT_Move(_target, _settings);
        }
    }
}
