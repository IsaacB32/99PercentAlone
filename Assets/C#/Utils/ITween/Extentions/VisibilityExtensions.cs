using UnityEngine;

namespace ITween
{
    using Internal;
    
    public static class VisibilityExtensions
    {
        public static VisibilityTween IT_Move(this Transform transform, Transform toTransform, TweenSettings_Visibility settings)
        {
            Vector3 fromPos = transform.position;
            Quaternion fromRot = transform.rotation;
            Vector3 fromScale = transform.localScale;

            Vector3 targetPos = toTransform.position;
            Quaternion targetRot = toTransform.rotation;
            Vector3 targetScale = toTransform.localScale;

            UnconfiguredTween visible = ITweenManager.Value(transform, 0f, 1f,
                t =>
                {
                    transform.position = Vector3.LerpUnclamped(fromPos, targetPos, t);
                    transform.rotation = Quaternion.SlerpUnclamped(fromRot, targetRot, t);
                    transform.localScale = Vector3.LerpUnclamped(fromScale, targetScale, t);
                }
            );

            return new VisibilityTween(transform, settings, visible);
        }
        
        public static VisibilityTween IT_MoveLocal(this Transform transform, Transform toTransform, TweenSettings_Visibility settings)
        {
            Vector3 fromPos = transform.localPosition;
            Quaternion fromRot = transform.localRotation;
            Vector3 fromScale = transform.localScale;

            Vector3 targetPos = toTransform.localPosition;
            Quaternion targetRot = toTransform.localRotation;
            Vector3 targetScale = toTransform.localScale;

            UnconfiguredTween visible = ITweenManager.Value(transform, 0f, 1f,
                t =>
                {
                    transform.localPosition = Vector3.LerpUnclamped(fromPos, targetPos, t);
                    transform.localRotation = Quaternion.SlerpUnclamped(fromRot, targetRot, t);
                    transform.localScale = Vector3.LerpUnclamped(fromScale, targetScale, t);
                }
            );

            return new VisibilityTween(transform, settings, visible);
        }
    }
}
