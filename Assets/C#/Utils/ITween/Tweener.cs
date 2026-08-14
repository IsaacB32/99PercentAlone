using System;
using System.Collections.Generic;
using UnityEngine;

namespace ITween
{
    public static class Tweener
    {
        private class ITweenRunner : MonoBehaviour
        {
#pragma warning disable UAC1001
            public List<Tween> ActiveTweens = new List<Tween>();
#pragma warning restore UAC1001

            private void Update()
            {
                for (int i = ActiveTweens.Count - 1; i >= 0; i--)
                {
                    ActiveTweens[i].Update(Time.deltaTime);
                    if (!ActiveTweens[i].IsAlive) ActiveTweens.RemoveAt(i);
                }
            }

            private void OnDestroy()
            {
                foreach (Tween tween in ActiveTweens)
                {
                    tween.Kill();
                }
                ActiveTweens.Clear();
            }
        }
        
        private static ITweenRunner _runner;
        private static ITweenRunner Runner
        {
            get
            {
                if (_runner == null)
                {
                    GameObject runner = new GameObject("TweenRunner");
                    UnityEngine.Object.DontDestroyOnLoad(runner);
                    _runner = runner.AddComponent<ITweenRunner>();
                }
                return _runner;
            }
        }
        
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void ResetOnDomainReload()
        {
            _runner = null;
        }
#endif
        
        public static Tween Value(
            UnityEngine.Object target,
            float from, 
            float to, 
            float duration, 
            Action<float> onUpdate, 
            Action onComplete = null, 
            EasingType easingType = EasingType.OutCubic)
        {
            var tween = new Tween(
                target,
                duration,
                t => onUpdate(Mathf.Lerp(from, to, t)),
                onComplete,
                Easing.GetEasingFunction(easingType)
            );
            Runner.ActiveTweens.Add(tween);
            return tween;
        }
    }
}
