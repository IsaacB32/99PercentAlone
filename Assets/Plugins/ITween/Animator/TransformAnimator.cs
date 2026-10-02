using NaughtyAttributes;
using UnityEngine;

namespace ITween.Animator
{
    using Internal;
    
    /// <summary>
    /// Moves transform to target
    /// </summary>
    public class TransformAnimator : TweenAnimator
    {
        [SerializeField, Required("A transform is required")] private Transform _transformTarget;
        [SerializeField, Required("Target is Required!")] protected Transform _target;
        [SerializeField] protected TweenSettings_Visibility _settings;

        private TransformSnapshot _origin;
        
        protected override VisibilityTween InitializeTween()
        {
            _origin = _transformTarget.ToSnapshot(isLocal: true);
            UnconfiguredTween visible = ITManager.IT_Value(_target, 0f, 1f,
                t =>
                {
                    _transformTarget.localPosition = Vector3.LerpUnclamped(_origin.position, _target.localPosition, t);
                    _transformTarget.localRotation = Quaternion.SlerpUnclamped(_origin.rotation, _target.localRotation, t);
                    _transformTarget.localScale = Vector3.LerpUnclamped(_origin.scale, _target.localScale, t);
                });
            
            UnconfiguredTween hidden = ITManager.IT_Value(_target, 0f, 1f,
                t =>
                {
                    _transformTarget.localPosition = Vector3.LerpUnclamped(_target.localPosition, _origin.position, t);
                    _transformTarget.localRotation = Quaternion.SlerpUnclamped(_target.localRotation, _origin.rotation, t);
                    _transformTarget.localScale = Vector3.LerpUnclamped(_target.localScale, _origin.scale, t);
                });
            return new VisibilityTween(_target, _settings, visible, hidden);
        }

        protected override void AssignType() => ImplicitAssignType(ref _transformTarget, gameObject);
    }
}