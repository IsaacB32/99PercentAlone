using UnityEngine;
using UnityEngine.UI;

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

            UnconfiguredTween visible = ITManager.Value(transform, 0f, 1f,
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

            UnconfiguredTween visible = ITManager.Value(transform, 0f, 1f,
                t =>
                {
                    transform.localPosition = Vector3.LerpUnclamped(fromPos, targetPos, t);
                    transform.localRotation = Quaternion.SlerpUnclamped(fromRot, targetRot, t);
                    transform.localScale = Vector3.LerpUnclamped(fromScale, targetScale, t);
                }
            );

            return new VisibilityTween(transform, settings, visible);
        }

        public static VisibilityTween IT_Alpha(this Graphic graphic, float toAlpha, TweenSettings_Visibility settings)
        {
            float fromAlpha = graphic.color.a;
            
            UnconfiguredTween visible = ITManager.Value(graphic, 0f, 1f,
                t =>
                {
                    Color tmp = graphic.color;
                    tmp.a = Mathf.LerpUnclamped(toAlpha, fromAlpha, t);
                    graphic.color = tmp;
                }
            );
            
            UnconfiguredTween hidden = ITManager.Value(graphic, 0f, 1f,
                t =>
                {
                    Color tmp = graphic.color;
                    tmp.a = Mathf.LerpUnclamped(fromAlpha, toAlpha, t);
                    graphic.color = tmp;
                }
            );

            return new VisibilityTween(graphic, settings, visible, hidden);
        }
        
        public static VisibilityTween IT_Alpha(this CanvasGroup group, float toAlpha, TweenSettings_Visibility settings)
        {
            float fromAlpha = group.alpha;
            
            UnconfiguredTween visible = ITManager.Value(group, 0f, 1f,
                t =>
                {
                    group.alpha = Mathf.LerpUnclamped(toAlpha, fromAlpha, t);
                }
            );
            
            UnconfiguredTween hidden = ITManager.Value(group, 0f, 1f,
                t =>
                {
                    group.alpha = Mathf.LerpUnclamped(fromAlpha, toAlpha, t);
                }
            );

            return new VisibilityTween(group, settings, visible, hidden);
        }
    }
}
