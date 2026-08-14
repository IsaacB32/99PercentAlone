using System;
using UnityEngine;

namespace ITween
{
    public static class TweenExtensions
    {
        public static Tween IT_Fade(this SpriteRenderer sr, float toAlpha, float duration, 
            Action onComplete = null, 
            EasingType easingType = EasingType.OutCubic)
        {
            float fromAlpha = sr.color.a;
            return Tweener.Value(
                sr,
                fromAlpha,
                toAlpha,
                duration,
                alpha =>
                {
                    Color color = sr.color;
                    color.a = alpha;
                    sr.color = color;
                }, 
                onComplete,
                easingType
                );
        }

        public static Tween IT_Move(this Transform transform, Transform toTransform, float duration,
            Action onComplete = null,
            EasingType easingType = EasingType.OutCubic)
        {
            Vector3 fromPos = transform.position;
            Quaternion fromRot = transform.rotation;
            Vector3 fromScale = transform.localScale;

            Vector3 targetPos = toTransform.position;
            Quaternion targetRot = toTransform.rotation;
            Vector3 targetScale = toTransform.localScale;
            
            return Tweener.Value(transform, 0f, 1f, duration,
                t =>
                {
                    transform.position = Vector3.Lerp(fromPos, targetPos, t);
                    transform.rotation = Quaternion.Slerp(fromRot, targetRot, t);
                    transform.localScale = Vector3.Lerp(fromScale, targetScale, t);
                },
                onComplete,
                easingType
            );
        }
        
        public static Tween IT_MoveRef(this Transform transform, Transform toTransform, float duration,
            Action onComplete = null,
            EasingType easingType = EasingType.OutCubic)
        {
            Vector3 fromPos = transform.position;
            Quaternion fromRot = transform.rotation;
            Vector3 fromScale = transform.localScale;
            return Tweener.Value(transform, 0f, 1f, duration,
                t =>
                {
                    transform.position = Vector3.Lerp(fromPos, toTransform.position, t);
                    transform.rotation = Quaternion.Slerp(fromRot, toTransform.rotation, t);
                    transform.localScale = Vector3.Lerp(fromScale, toTransform.localScale, t);
                },
                onComplete,
                easingType
            );
        }
        
        public static Tween IT_Move(this Transform transform, Vector3 toPos, float duration,
            Action onComplete = null,
            EasingType easingType = EasingType.OutCubic)
        {
            Vector3 fromPos = transform.position;
            return Tweener.Value(transform, 0f, 1f, duration,
                t =>
                {
                    transform.position = Vector3.Lerp(fromPos, toPos, t);
                },
                onComplete,
                easingType
            );
        }
    }

}