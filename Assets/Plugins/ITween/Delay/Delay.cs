using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ITween
{
    public static class Delay
    {
        private class DelayRunner : MonoBehaviour
        {
            private const int DEFAULT_DELAY_CAPACITY = 50;
            
            //todo: find a way to collect all running coroutines and destroy them as needed
            private List<Coroutine> _allRunners = new List<Coroutine>(DEFAULT_DELAY_CAPACITY);
            
            public void StartTimer(float time, Action onComplete)
            {
                StartCoroutine(Timer());
                return;
                
                IEnumerator Timer()
                {
                    yield return new WaitForSeconds(time);
                    onComplete.Invoke();
                }
            }

            public void StartTimerRealtime(float time, Action onComplete)
            {
                StartCoroutine(TimerRealtime());
                return;
                
                IEnumerator TimerRealtime()
                {
                    yield return new WaitForSecondsRealtime(time);
                    onComplete.Invoke();
                }
            }

            public void StartNextFrame(Action onComplete)
            {
                StartCoroutine(NextFrame());
                return;
                
                IEnumerator NextFrame()
                {
                    yield return new WaitForEndOfFrame();
                    onComplete.Invoke();
                }
            }
            
            public void StartNextFrame(int amount, Action stepAction, Action onComplete)
            {
                int elapsed = 0;
                StartCoroutine(NextFrame());
                return;
                
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

            public void StartWaitUntil(Func<bool> pred, Action onComplete)
            {
                StartCoroutine(When());
                return;
                
                IEnumerator When()
                {
                    yield return new WaitUntil(pred);
                    onComplete.Invoke();
                }
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
        public static void Wait(float timer, Action onComplete)
        {
            Runner.StartTimer(timer, onComplete);
        }
        
        /// <summary>
        /// Wait Realtime
        /// </summary>
        public static void WaitRealtime(float timer, Action onComplete)
        {
            Runner.StartTimerRealtime(timer, onComplete);
        }
        
        /// <summary>
        /// Invoke action on next frame
        /// </summary>
        public static void WaitForNextFrame(Action onComplete)
        {
            Runner.StartNextFrame(onComplete);
        }

        /// <summary>
        /// Invoke action repeated each frame
        /// </summary>
        /// <param name="amount">amount of frames to run</param>
        /// <param name="stepAction">action invoked each frame</param>
        /// <param name="onComplete">action on complete</param>
        public static void WaitForNextFrame(int amount, Action stepAction, Action onComplete)
        {
            Runner.StartNextFrame(amount, stepAction, onComplete);
        }
        
        /// <summary>
        /// Wait until a condition is met
        /// </summary>
        public static void WaitUntil(Func<bool> pred, Action onComplete)
        {
            Runner.StartWaitUntil(pred, onComplete);
        }
        
    }
}
