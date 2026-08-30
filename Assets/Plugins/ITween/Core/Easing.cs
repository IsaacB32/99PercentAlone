using System;
using UnityEngine;

namespace ITween
{
    /// <summary>
    /// I made my own tweening library because why not, math functions from https://gamedevcheatsheet.com/easing
    /// </summary>
    public static class Easing
    {
        public static Func<float, float> GetEasingFunction(EasingType type, float overshoot, AnimationCurve customCurve)
        {
            return type switch
            {
                EasingType.Linear => EaseLinear,
                EasingType.InSine => EaseInSine,
                EasingType.OutSine => EaseOutSine,
                EasingType.InOutSine => EaseInOutSine,
                EasingType.InQuad => EaseInQuad,
                EasingType.OutQuad => EaseOutQuad,
                EasingType.InOutQuad => EaseInOutQuad,
                EasingType.InCubic => EaseInCubic,
                EasingType.OutCubic => EaseOutCubic,
                EasingType.InOutCubic => EaseInOutCubic,
                EasingType.InQuart => EaseInQuart,
                EasingType.OutQuart => EaseOutQuart,
                EasingType.InOutQuart => EaseInOutQuart,
                EasingType.InExpo => EaseInExpo,
                EasingType.OutExpo => EaseOutExpo,
                EasingType.InOutExpo => EaseInOutExpo,
                EasingType.InCirc => EaseInCirc,
                EasingType.OutCirc => EaseOutCirc,
                EasingType.InOutCirc => EaseInOutCirc,
                EasingType.InBack => t => EaseInBack(t, overshoot),
                EasingType.OutBack => t => EaseOutBack(t, overshoot),
                EasingType.InOutBack => t => EaseInOutBack(t, overshoot),
                EasingType.InElastic => EaseInElastic,
                EasingType.OutElastic => EaseOutElastic,
                EasingType.InOutElastic => EaseInOutElastic,
                EasingType.InBounce => EaseInBounce,
                EasingType.OutBounce => EaseOutBounce,
                EasingType.InOutBounce => EaseInOutBounce,
                EasingType.Custom => customCurve.Evaluate,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
        
        public static Func<float, float> GetInvertedEasingFunction(EasingType type, float overshoot, AnimationCurve customCurve)
        {
            return type switch
            {
                EasingType.Linear => EaseLinear,
                EasingType.InSine => EaseOutSine,
                EasingType.OutSine => EaseInSine,
                EasingType.InOutSine => EaseInOutSine,
                EasingType.InQuad => EaseOutQuad,
                EasingType.OutQuad => EaseInQuad,
                EasingType.InOutQuad => EaseInOutQuad,
                EasingType.InCubic => EaseOutCubic,
                EasingType.OutCubic => EaseInCubic,
                EasingType.InOutCubic => EaseInOutCubic,
                EasingType.InQuart => EaseOutQuart,
                EasingType.OutQuart => EaseInQuart,
                EasingType.InOutQuart => EaseInOutQuart,
                EasingType.InExpo => EaseOutExpo,
                EasingType.OutExpo => EaseInExpo,
                EasingType.InOutExpo => EaseInOutExpo,
                EasingType.InCirc => EaseOutCirc,
                EasingType.OutCirc => EaseInCirc,
                EasingType.InOutCirc => EaseInOutCirc,
                EasingType.InBack => t => EaseOutBack(t, overshoot),
                EasingType.OutBack => t => EaseInBack(t, overshoot),
                EasingType.InOutBack => t => EaseInOutBack(t, overshoot),
                EasingType.InElastic => EaseOutElastic,
                EasingType.OutElastic => EaseInElastic,
                EasingType.InOutElastic => EaseInOutElastic,
                EasingType.InBounce => EaseOutBounce,
                EasingType.OutBounce => EaseInBounce,
                EasingType.InOutBounce => EaseInOutBounce,
                EasingType.Custom => invertedCustom,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            float invertedCustom(float t)
            {
                return 1f - customCurve.Evaluate(1f - t);
            }
        }

        #region Math

        public static float EaseLinear(float t)
        {
            return t;
        }
        
        #region Sine

        public static float EaseInSine(float t)
        {
            return 1f - Mathf.Cos(t * Mathf.PI / 2f);
        }

        public static float EaseOutSine(float t)
        {
            return Mathf.Sin(t * Mathf.PI / 2f);
        }

        public static float EaseInOutSine(float t)
        {
            return -(Mathf.Cos(Mathf.PI * t) - 1f) / 2f;
        }

        #endregion

        #region Quad

        public static float EaseInQuad(float t)
        {
            return t * t;
        }

        public static float EaseOutQuad(float t)
        {
            return 1f - (1f - t) * (1f - t);
        }

        public static float EaseInOutQuad(float t)
        {
            return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
        }

        #endregion

        #region Cubic

        public static float EaseInCubic(float t)
        {
            return t * t * t;
        }

        public static float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        public static float EaseInOutCubic(float t)
        {
            return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
        }

        #endregion

        #region Quart

        public static float EaseInQuart(float t)
        {
            return t * t * t * t;
        }

        public static float EaseOutQuart(float t)
        {
            return 1f - Mathf.Pow(1f - t, 4f);
        }

        public static float EaseInOutQuart(float t)
        {
            return t < 0.5f ? 8f * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 4f) / 2f;
        }

        #endregion

        #region Expo

        public static float EaseInExpo(float t)
        {
            return t == 0f ? 0f : Mathf.Pow(2f, 10f * t - 10f);
        }

        public static float EaseOutExpo(float t)
        {
            return t == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
        }

        public static float EaseInOutExpo(float t)
        {
            return t == 0f
                ? 0f
                : t == 1f
                    ? 1f
                    : t < 0.5f
                        ? Mathf.Pow(2f, 20f * t - 10f) / 2f
                        : (2f - Mathf.Pow(2f, -20f * t + 10f)) / 2f;
        }

        #endregion

        #region Circ

        public static float EaseInCirc(float t)
        {
            return 1f - Mathf.Sqrt(1f - Mathf.Pow(t, 2f));
        }

        public static float EaseOutCirc(float t)
        {
            return Mathf.Sqrt(1f - Mathf.Pow(t - 1f, 2f));
        }

        public static float EaseInOutCirc(float t)
        {
            return t < 0.5f
                ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * t, 2f))) / 2f
                : (Mathf.Sqrt(1f - Mathf.Pow(-2f * t + 2f, 2f)) + 1f) / 2f;
        }

        #endregion

        #region Back

        public static float EaseInBack(float t, float overshoot)
        { 
            float c3 = overshoot + 1f;
            return c3 * t * t * t - overshoot * t * t;
        }

        public static float EaseOutBack(float t, float overshoot)
        {
            float c3 = overshoot + 1f;
            return 1f + c3 * Mathf.Pow(t - 1f, 3f) + overshoot * Mathf.Pow(t - 1f, 2f);
        }

        public static float EaseInOutBack(float t, float overshoot)
        {
            float c2 = overshoot * 1.525f;
            return t < 0.5f
                ? (Mathf.Pow(2f * t, 2f) * ((c2 + 1f) * 2f * t - c2)) / 2f
                : (Mathf.Pow(2f * t - 2f, 2f) * ((c2 + 1f) * (t * 2f - 2f) + c2) + 2f) / 2f;
        }

        #endregion

        #region Elastic

        public static float EaseInElastic(float t)
        {
            const float c4 = 2f * Mathf.PI / 3f;
            return t == 0f ? 0f
                : t == 1f ? 1f
                : -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sin((t * 10f - 10.75f) * c4);
        }

        public static float EaseOutElastic(float t)
        {
            const float c4 = 2f * Mathf.PI / 3f;
            return t == 0f ? 0f
                : t == 1f ? 1f
                : Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
        }

        public static float EaseInOutElastic(float t)
        {
            const float c5 = 2f * Mathf.PI / 4.5f;
            return t == 0f
                ? 0f
                : t == 1f
                    ? 1f
                    : t < 0.5f
                        ? -(Mathf.Pow(2f, 20f * t - 10f) * Mathf.Sin((20f * t - 11.125f) * c5)) / 2f
                        : Mathf.Pow(2f, -20f * t + 10f) * Mathf.Sin((20f * t - 11.125f) * c5) / 2f + 1f;
        }

        #endregion

        #region Bounce

        public static float EaseInBounce(float t)
        {
            return 1f - EaseOutBounce(1f - t);
        }

        public static float EaseOutBounce(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;
            if (t < 1f / d1) return n1 * t * t;
            if (t < 2f / d1) return n1 * (t -= 1.5f / d1) * t + 0.75f;
            if (t < 2.5f / d1) return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            return n1 * (t -= 2.625f / d1) * t + 0.984375f;
        }

        public static float EaseInOutBounce(float t)
        {
            return t < 0.5f
                ? (1f - EaseOutBounce(1f - 2f * t)) / 2f
                : (1f + EaseOutBounce(2f * t - 1f)) / 2f;
        }

        #endregion

        #endregion
    }

    public enum EasingType
    {
        Linear,
        InSine,
        OutSine,
        InOutSine,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc,
        InBack,
        OutBack,
        InOutBack,
        InElastic,
        OutElastic,
        InOutElastic,
        InBounce,
        OutBounce,
        InOutBounce,
        Custom
    }
}