using System;
using System.Collections;
using UnityEngine;

namespace ITween
{
    public static class Delay
    {
        private class DelayRunner : MonoBehaviour
        {
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

        public static void Wait(float timer, Action onComplete)
        {
            Runner.StartTimer(timer, onComplete);
        }
        
        public static void WaitRealtime(float timer, Action onComplete)
        {
            Runner.StartTimerRealtime(timer, onComplete);
        }
        
        public static void WaitForNextFrame(Action onComplete)
        {
            Runner.StartNextFrame(onComplete);
        }
        
        public static void WaitUntil(Func<bool> pred, Action onComplete)
        {
            Runner.StartWaitUntil(pred, onComplete);
        }
        
    }
}
