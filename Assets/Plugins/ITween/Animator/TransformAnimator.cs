using UnityEngine;
using NaughtyAttributes;

namespace ITween.Animator
{
    /// <summary>
    /// Moves transform to target
    /// </summary>
    public class TransformAnimator : TweenAnimator
    {
        [SerializeField, Tooltip("By default use attached gameObject")] private bool _overrideTarget;
        [SerializeField, ShowIf(nameof(_overrideTarget)), Required("A transform is required")] private Transform _transformTarget;
        [SerializeField, Required("Target is Required!")] protected Transform _target;
        [SerializeField] protected TweenSettings _settings;
        
        protected override Tween InitializeTween()
        {
            Tween t = _overrideTarget ? _transformTarget.IT_Move(_target, _settings) : transform.IT_Move(_target, _settings);
            return t;
        }
    }
}
