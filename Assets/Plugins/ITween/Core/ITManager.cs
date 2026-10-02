using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
[assembly: InternalsVisibleTo("ITween.Editor")]

namespace ITween
{
    using Internal;
    
    /// <summary>
    /// Static class responsible for running Tweens
    /// </summary>
    public static class ITManager
    {
        public static int TweenCounter = 0;
        
        internal class ITweenRunner : MonoBehaviour
        {
            private const int INITIAL_CAPACITY = 50;
            private Dictionary<int, ITween> _activeTweens { get; } = new Dictionary<int, ITween>(INITIAL_CAPACITY);

            private List<ITween> _updateBuffer = new List<ITween>(); //buffer to separate Killed Tweens from Active Tweens
            private List<int> _toRemove = new List<int>();

            [SerializeField] private int _activeAmount;

            public bool AddTween(ITween t)
            {
                return _activeTweens.TryAdd(t.IDKey, t);
            }

            public bool RemoveTween(ITween t)
            {
                return _activeTweens.Remove(t.IDKey);
            }
            
            private void Update()
            {
                _updateBuffer.Clear();
                _updateBuffer.AddRange(_activeTweens.Values);
                
                foreach (ITween tween in _updateBuffer)
                {
                    tween.Update();
                    if (!tween.IsAlive) _toRemove.Add(tween.IDKey);
                }

                for (int i = _toRemove.Count - 1; i >= 0; i--) _activeTweens.Remove(_toRemove[i]);
                _toRemove.Clear();

                _activeAmount = _activeTweens.Count;
            }

            private void OnDestroy()
            {
                KillAll();
            }

            public void KillAll()
            {
                _updateBuffer.Clear();
                _updateBuffer.AddRange(_activeTweens.Values);
                
                foreach (ITween tween in _updateBuffer)
                {
                    tween.Kill(ignoreFlags: false);
                }
                
                _toRemove.Clear();
                _updateBuffer.Clear();
                _activeTweens.Clear();
                TweenCounter = 0;
            }
        }
        
        private static ITweenRunner _runner;
        private static ITweenRunner Runner
        {
            get
            {
                if (_runner == null)
                {
                    TweenCounter = 0;
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
            TweenCounter = 0;
            _runner = null;
        }
#endif
        
        /// <summary>
        /// Creates a new Tween with TweenSettings
        /// </summary>
        public static Tween IT_Value(
            UnityEngine.Object target,
            float from, 
            float to, 
            ITweenSettings settings, 
            Action<float> onUpdate, 
            Action onComplete = null)
        {
            Tween tween = new Tween(
                target,
                settings,
                t => onUpdate(Mathf.LerpUnclamped(from, to, t)),
                onComplete
            );
            return tween;
        }
        
        /// <summary>
        /// Creates a new Tween with Duration and EasingType
        /// </summary>
        public static Tween IT_Value(
            UnityEngine.Object target,
            float from, 
            float to, 
            float duration,
            EasingType easingType,
            Action<float> onUpdate, 
            Action onComplete = null)
        {
            Tween tween = new Tween(
                target,
                new TweenSettings(duration, easingType),
                t => onUpdate(Mathf.LerpUnclamped(from, to, t)),
                onComplete
            );
            return tween;
        }
        
        /// <summary>
        /// Create an Unconfigured Tween with no settings
        /// </summary>
        public static UnconfiguredTween IT_Value(
            UnityEngine.Object target,
            float from, 
            float to, 
            Action<float> onUpdate)
        {
            UnconfiguredTween tween = new UnconfiguredTween(
                target,
                t => onUpdate(Mathf.LerpUnclamped(from, to, t))
            );
            return tween;
        }

        /// <summary>
        /// Add a Tween to the Runner
        /// </summary>
        internal static bool StartTween(ITween tween)
        {
            return Runner.AddTween(tween);
        }

        /// <summary>
        /// Remove a Tween from the Runner
        /// </summary>
        internal static bool StopTween(ITween tween)
        {
            return Runner.RemoveTween(tween);
        }

        public static void KillAllTween()
        {
            Runner.KillAll();
        }
    }
}
