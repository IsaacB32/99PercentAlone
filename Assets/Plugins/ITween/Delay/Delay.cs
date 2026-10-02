using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace ITween
{
    public static class Delay
    {
        private class DelayRunner : MonoBehaviour
        {
            private Dictionary<ProtectedCoroutine, Coroutine> _allRunners = new Dictionary<ProtectedCoroutine, Coroutine>();
            private int _nextDelayIndex = 0;
            
            public ProtectedCoroutine StartTimer(float time, [NotNull] Action onComplete)
            {
                return AsProtectedCoroutine(Timer());
                
                IEnumerator Timer()
                {
                    yield return new WaitForSeconds(time);
                    onComplete.Invoke();
                }
            }
            
            //===== Coroutines =====

            public ProtectedCoroutine StartTimerRealtime(float time, [NotNull] Action onComplete)
            {
                return AsProtectedCoroutine(TimerRealtime());
                
                IEnumerator TimerRealtime()
                {
                    yield return new WaitForSecondsRealtime(time);
                    onComplete.Invoke();
                }
            }

            public ProtectedCoroutine StartNextFrame([NotNull] Action onComplete)
            {
                return AsProtectedCoroutine(NextFrame());
                
                IEnumerator NextFrame()
                {
                    yield return new WaitForEndOfFrame();
                    onComplete.Invoke();
                }
            }
            
            public ProtectedCoroutine StartNextFrame(int amount, [NotNull] Action stepAction, [NotNull] Action onComplete)
            {
                int elapsed = 0;
                return AsProtectedCoroutine(NextFrame());
                
                IEnumerator NextFrame()
                {
                    while (elapsed < amount)
                    {
                        elapsed++;
                        stepAction.Invoke();
                        yield return new WaitForEndOfFrame();
                    }
                    onComplete.Invoke();
                }
            }

            public ProtectedCoroutine StartWaitUntil(Func<bool> pred, [NotNull] Action onComplete)
            {
                return AsProtectedCoroutine(When());
                
                IEnumerator When()
                {
                    yield return new WaitUntil(pred);
                    onComplete.Invoke();
                }
            }
            
            //===== Protected =====

            public ProtectedCoroutine AsProtectedCoroutine([NotNull] IEnumerator enumerator, Action onComplete = null)
            {
                int id = ++_nextDelayIndex;
                ProtectedCoroutine key = new ProtectedCoroutine(id);
                Coroutine coroutine = StartCoroutine(CleanUp());

                _allRunners.Add(key, coroutine);
                return key;

                IEnumerator CleanUp()
                {
                    yield return enumerator;
                    _allRunners.Remove(key);
                    onComplete?.Invoke();
                }
            }

            public void StopAsProtectedCoroutine(ProtectedCoroutine coroutine)
            {
                if (_allRunners.TryGetValue(coroutine, out Coroutine target))
                {
                    StopCoroutine(target);
                    _allRunners.Remove(coroutine);
                }
            }

            public void DelayStopAll()
            {
                foreach (ProtectedCoroutine coroutine in _allRunners.Keys)
                {   
                    StopAsProtectedCoroutine(coroutine);
                }
                _allRunners.Clear();
            }

            public bool ProtectedCoroutineRunning(ProtectedCoroutine key)
            {
                return _allRunners.TryGetValue(key, out _);
            }
        }
        
        private static DelayRunner _runner;
        private static DelayRunner Runner
        {
            get
            {
                if (_runner == null)
                {
                    GameObject runner = new GameObject("DelayRunner");
                    UnityEngine.Object.DontDestroyOnLoad(runner);
                    _runner = runner.AddComponent<DelayRunner>();
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

        /// <summary>
        /// Standard WaitForSeconds timer
        /// </summary>
        public static void Wait(float timer, [NotNull] Action onComplete)
        {
            Runner.StartTimer(timer, onComplete);
        }
        
        /// <summary>
        /// Wait Realtime
        /// </summary>
        public static void WaitRealtime(float timer, [NotNull] Action onComplete)
        {
            Runner.StartTimerRealtime(timer, onComplete);
        }
        
        /// <summary>
        /// Invoke action on next frame
        /// </summary>
        public static void WaitForNextFrame([NotNull] Action onComplete)
        {
            Runner.StartNextFrame(onComplete);
        }

        /// <summary>
        /// Invoke action repeated each frame
        /// </summary>
        /// <param name="amount">amount of frames to run</param>
        /// <param name="stepAction">action invoked each frame</param>
        /// <param name="onComplete">action on complete</param>
        public static void WaitForNextFrame(int amount, [NotNull] Action stepAction, [NotNull] Action onComplete)
        {
            Runner.StartNextFrame(amount, stepAction, onComplete);
        }
        
        /// <summary>
        /// Wait until a condition is met
        /// </summary>
        public static void WaitUntil(Func<bool> pred, [NotNull] Action onComplete)
        {
            Runner.StartWaitUntil(pred, onComplete);
        }

        /// <summary>
        /// Start a coroutine that can be stopped by Delay
        /// </summary>
        /// <param name="enumerator">IEnumerator to run</param>
        /// <param name="onComplete">Action fired when IEnumerator is finished</param>
        /// <returns>ID of currently running coroutine</returns>
        public static ProtectedCoroutine StartProtectedCoroutine(IEnumerator enumerator, Action onComplete = null)
        {
            return Runner.AsProtectedCoroutine(enumerator, onComplete);
        }

        /// <summary>
        /// Stop provided ProtectedCoroutine
        /// </summary>
        public static void StopProtectedCoroutine(ProtectedCoroutine coroutine)
        {
            Runner.StopAsProtectedCoroutine(coroutine);
        }

        /// <summary>
        /// Stop all ProtectedCoroutines
        /// </summary>
        public static void StopAll()
        {
            Runner.DelayStopAll();
        }

        public static bool IsProtectedCoroutineRunning(ProtectedCoroutine key)
        {
            return Runner.ProtectedCoroutineRunning(key);
        }
    }
    
    public readonly struct ProtectedCoroutine : IEquatable<ProtectedCoroutine>
    {
        private readonly int _id;

        /// <summary>
        /// Is the Coroutine running
        /// </summary>
        public bool IsRunning => _id != 0 && Delay.IsProtectedCoroutineRunning(this);

        public void StopIfRunning()
        {
            if (IsRunning) Delay.StopProtectedCoroutine(this);
        }

        internal ProtectedCoroutine(int index)
        {
            _id = index;
        }

        public bool Equals(ProtectedCoroutine other) { return _id == other._id; }
        public override bool Equals(object obj) { return obj is ProtectedCoroutine other && Equals(other); }
        public override int GetHashCode() { return _id; }
    }
}
