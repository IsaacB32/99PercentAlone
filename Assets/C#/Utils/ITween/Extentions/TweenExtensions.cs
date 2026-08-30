using System;
using UnityEngine;

namespace ITween
{
    using Pathing;
    using Internal;
    
    public static class TweenExtensions
    {
        /// <summary>
        /// Fade the alpha of a sprite renderer + settings
        /// </summary>
        public static Tween IT_Fade(this SpriteRenderer sr, float toAlpha, TweenSettings settings, Action onComplete = null)
        {
            float fromAlpha = sr.color.a;
            return ITManager.Value(
                sr,
                fromAlpha,
                toAlpha,
                settings,
                alpha =>
                {
                    Color color = sr.color;
                    color.a = alpha;
                    sr.color = color;
                }, 
                onComplete
            );
        }
        
        /// <summary>
        /// Move one transform to the original transform of another
        /// </summary>
        public static Tween IT_Move(this Transform transform, Transform toTransform, ITweenSettings settings, Action onComplete = null)
        {
            Vector3 fromPos = transform.position;
            Quaternion fromRot = transform.rotation;

            Vector3 targetPos = toTransform.position;
            Quaternion targetRot = toTransform.rotation;
            
            return ITManager.Value(transform, 0f, 1f, settings,
                t =>
                {
                    transform.position = Vector3.LerpUnclamped(fromPos, targetPos, t);
                    transform.rotation = Quaternion.SlerpUnclamped(fromRot, targetRot, t);
                },
                onComplete
            );
        }
        
        /// <summary>
        /// Move the transform position
        /// </summary>
        public static Tween IT_Move(this Transform transform, Vector3 toPos, ITweenSettings settings, Action onComplete = null)
        {
            Vector3 fromPos = transform.position;
            return ITManager.Value(transform, 0f, 1f, settings,
                t =>
                {
                    transform.position = Vector3.LerpUnclamped(fromPos, toPos, t);
                },
                onComplete
            );
        }
        
        /// <summary>
        /// Move one transform + scale to the original transform + scale of another
        /// </summary>
        public static Tween IT_MoveScale(this Transform transform, Transform toTransform, ITweenSettings settings, Action onComplete = null)
        {
            Vector3 fromPos = transform.position;
            Quaternion fromRot = transform.rotation;
            Vector3 fromScale = transform.localScale;

            Vector3 targetPos = toTransform.position;
            Quaternion targetRot = toTransform.rotation;
            Vector3 targetScale = toTransform.localScale;
            
            return ITManager.Value(transform, 0f, 1f, settings,
                t =>
                {
                    transform.position = Vector3.LerpUnclamped(fromPos, targetPos, t);
                    transform.rotation = Quaternion.SlerpUnclamped(fromRot, targetRot, t);
                    transform.localScale = Vector3.LerpUnclamped(fromScale, targetScale, t);
                },
                onComplete
            );
        }
        
        public static Tween IT_MovePath(this Transform transform, Vector3[] points, TweenSettings_Path settings, 
            Action<int> onStepComplete = null,
            Action onComplete = null)
        {
            ITween_VectorPath fromPos = new ITween_VectorPath { Pos = transform.position };
            ITween_VectorPath[] pathPoints = Array.ConvertAll(points, input => new ITween_VectorPath {Pos = input});

            TweenPath<ITween_VectorPath> path = new TweenPath<ITween_VectorPath>(fromPos, pathPoints);
            return ITManager.Value(transform, 0f, 1f, settings,
                t =>
                {
                    TweenPath<ITween_VectorPath>.PathPoint target = path.FindTarget(t, onStepComplete);
                    if (target.IsEnd)
                    {
                        transform.position = target.Point.Pos;
                        return;
                    }
                    transform.position = Vector3.LerpUnclamped(target.Point.Pos, target.Next.Point.Pos, target.GetRelativeProgress());
                },
                onComplete
            );
        }
        
        public static Tween IT_MovePath(this Transform transform, Transform[] points, TweenSettings_Path settings, 
            Action<int> onStepComplete = null,
            Action onComplete = null)
        {
            //if we are only moving the position then call a vector[] path instead
            if (settings.TransformType == TweenSettings_Path.PathTransformType.Position)
            {
                Vector3[] vectorPoints = Array.ConvertAll(points, input => input.position);
                return IT_MovePath(transform, vectorPoints, settings);
            }

            bool hasPos = settings.TransformType.HasFlag(TweenSettings_Path.PathTransformType.Position);
            bool hasRot = settings.TransformType.HasFlag(TweenSettings_Path.PathTransformType.Rotation);
            bool hasScale = settings.TransformType.HasFlag(TweenSettings_Path.PathTransformType.Scale);
            if (!hasPos && !hasRot && !hasScale)
            {
                throw new InvalidOperationException("attempting to MovePath with no transform properties allowed");
            }
            
            ITween_TransformPath fromPoint = new ITween_TransformPath { transform = transform};
            ITween_TransformPath[] pathPoints = Array.ConvertAll(points, input => new ITween_TransformPath {transform = input});

            TweenPath<ITween_TransformPath> path = new TweenPath<ITween_TransformPath>(fromPoint, pathPoints);
            return ITManager.Value(transform, 0f, 1f, settings,
                t =>
                {
                    TweenPath<ITween_TransformPath>.PathPoint target = path.FindTarget(t, onStepComplete);
                    Transform currentTargetTransform = ((ITween_TransformPath)target.Point).transform;
                    if (target.IsEnd)
                    {
                        if (hasPos) transform.position = currentTargetTransform.position;
                        if (hasRot) transform.rotation = currentTargetTransform.rotation;
                        if (hasScale) transform.localScale = currentTargetTransform.localScale;
                        return;
                    }
                    
                    //next assigned after IsEnd in-case of null
                    Transform nextTargetTransform = ((ITween_TransformPath)target.Next.Point).transform;

                    float relativeProgress = target.GetRelativeProgress();
                    if (hasPos) transform.position = Vector3.LerpUnclamped(currentTargetTransform.position, nextTargetTransform.position, relativeProgress);
                    if (hasRot) transform.rotation = Quaternion.SlerpUnclamped(currentTargetTransform.rotation, nextTargetTransform.rotation, relativeProgress);
                    if (hasScale)  transform.localScale = Vector3.LerpUnclamped(currentTargetTransform.localScale, nextTargetTransform.localScale, relativeProgress);
                },
                onComplete
            );
        }
    }

}