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
    public static class ITweenManager
    {
        internal class ITweenRunner : MonoBehaviour
        {
            private const int INITIAL_CAPACITY = 50;
            private Dictionary<int, Tween> _activeTweens { get; } = new Dictionary<int, Tween>(INITIAL_CAPACITY);

            private List<Tween> _updateBuffer = new List<Tween>(); //buffer to separate Killed Tweens from Active Tweens
            private List<int> _toRemove = new List<int>();

            [SerializeField] private int _activeAmount;

            public bool AddTween(Tween t)
            {
                return _activeTweens.TryAdd(t.IDKey, t);
            }

            public bool RemoveTween(Tween t)
            {
                return _activeTweens.Remove(t.IDKey);
            }
            
            private void Update()
            {
                _updateBuffer.Clear();
                _updateBuffer.AddRange(_activeTweens.Values);
                
                foreach (Tween tween in _updateBuffer)
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
                
                foreach (Tween tween in _updateBuffer)
                {
                    Tween.IT_Kill(tween);
                }
                
                _toRemove.Clear();
                _updateBuffer.Clear();
                _activeTweens.Clear();
                Tween.TweenCounter = 0;
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
            Tween.TweenCounter = 0;
            _runner = null;
        }
#endif
        
        /// <summary>
        /// Creates a new Tween with TweenSettings
        /// </summary>
        public static Tween Value(
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
        public static Tween Value(
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
        
        public static UnconfiguredTween Value(
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
        public static bool StartTween(Tween tween)
        {
            return Runner.AddTween(tween);
        }

        /// <summary>
        /// Remove a Tween from the Runner
        /// </summary>
        public static bool StopTween(Tween tween)
        {
            return Runner.RemoveTween(tween);
        }

        public static void KillAllTween()
        {
            Runner.KillAll();
        }
    }
}
