using System;
using JetBrains.Annotations;
using UnityEngine;

namespace ITween
{
    using Internal;
    
    /// <summary>
    /// Custom Tween 
    /// </summary>
    public sealed class Tween
    {
        public static int TweenCounter = 0; 
        
        //Core
        public int IDKey { get; private set; }  //identifier
        private float _elapsed;
        private float _delayElapsed;
        private float _loopDelayElapsed;
        private bool _isInverted;
        private bool _isForwards;
        private bool _isStartingDirectionForwards;
        private Func<bool> _pausePredicate;

        //Core Properties
        public bool IsAlive { get; private set; } = true;       //lifetime of the Tween
        public bool IsRunning { get; private set; } = false;    //is active
        public bool IsPaused { get; private set; } = false;     //is active under pause predicate

        //Target Reference + Callbacks
        private UnityEngine.Object _target;
        private event Action<float> _onUpdate;
        public event Action OnStart; //start playing
        public event Action OnStop; //stop playing
        public event Action OnLoopComplete; //loop step finished (there -> repeat, there + back -> ping-pong)
        public event Action OnComplete; //tween finished and killed
        private Action _onUnpause;

        //Tween Settings
        private ITweenSettings _settings;
        private TweenFlags _flags;
        private Func<float, float> _activeEasingFunction;
        private Func<float, float> _easingFunction, _invertedEasingFunction;
        private int _remainingLoops;
        
        private bool _isDead => !IsAlive || _target == null;

        //===== Constructor =====

        public Tween(
            [NotNull] UnityEngine.Object target,
            [NotNull] ITweenSettings settings,
            [NotNull] Action<float> onUpdate,
            Action onComplete = null
        )
        {
            _target = target;
            IDKey = TweenCounter++;
            
            _onUpdate = onUpdate;
            OnComplete += onComplete;
            _pausePredicate = null;
            
            _settings = settings;
            _flags = settings.Flags;

            _easingFunction = Easing.GetEasingFunction(settings.EaseType, settings.Overshoot, settings.CustomCurve);
            _invertedEasingFunction = Easing.GetInvertedEasingFunction(settings.EaseType, settings.Overshoot, settings.CustomCurve);
            
            if (_flags.InvertStartingDirection) InvertSelf();
            else _isForwards = !_isInverted;
            
            _activeEasingFunction = GetEasingFunction();
            _isStartingDirectionForwards = _isForwards;

            _loopDelayElapsed = settings.HangTime;
            _remainingLoops = settings.LoopCount;
            
            if (_flags.StartAutomatically) Start();
        }
        
        public Tween(Tween copy) : this(copy._target, copy._settings, copy._onUpdate, copy.OnComplete) { }
        
        //===== Update =====
        
        public void Update()
        {
            //check lifetime
            if (_isDead)
            {
                Kill(ignoreFlags: false);
                return;
            }
            
            float deltaTime = GetDeltaTime;
            
            //wait for pause time
            if (IsPaused)
            {
                if (_pausePredicate.Invoke())
                {
                    IsPaused = false;
                    _onUnpause?.Invoke();
                    _onUnpause = null;
                }
                return;
            }

            //wait for delay
            if (_delayElapsed < _settings.DelayTime)
            {
                _delayElapsed += deltaTime;
                return;
            }

            //wait for loop delay
            if (_loopDelayElapsed < _settings.HangTime)
            {
                _loopDelayElapsed += deltaTime;
                return;
            }

            //handle easing
            _elapsed += deltaTime;
            float t = Mathf.Clamp01(_elapsed / _settings.Duration);
            if (!_isForwards) t = 1 - t;
            float easedT = _activeEasingFunction.Invoke(t);
            
            _onUpdate.Invoke(easedT);

            //handle completion
            if (_elapsed >= _settings.Duration)
            {
                _remainingLoops--;
                if (_settings.LoopingType == LoopType.PingPong)
                {
                    //increase the loop count when finishing forwards so only counts one full ping-pong
                    if (_isForwards == _isStartingDirectionForwards)
                    {
                        _loopDelayElapsed = 0f; //start loopHangTime
                        _remainingLoops++;
                    }
                    else OnLoopComplete?.Invoke();
                    
                    InvertSelf();
                }

                if (_settings.LoopingType == LoopType.Single || _remainingLoops <= 0)
                {
                    _onUpdate.Invoke(1f);
                    OnComplete?.Invoke();
                    Restart();
                    return;
                }

                if (_settings.LoopingType == LoopType.Repeat)
                {
                    _loopDelayElapsed = 0f; //start loopHangTime
                    OnLoopComplete?.Invoke();
                }

                //account for the overflow when deltaTime is lage 
                do { _elapsed -= _settings.Duration; } while (_elapsed >= _settings.Duration); 
            }
        }
        
        //===== Control =====

        /// <summary>
        /// Start playing, add to ActiveTweens
        /// </summary>
        public Tween Start(Action onComplete = null)
        {
            if (!IsAlive) throw new Exception("Tween was killed before it could be started");
            if (IsPaused || IsRunning) return this;

            OnComplete += onComplete;
            
            IsRunning = true;
            OnStart?.Invoke();
            ITweenManager.StartTween(this);
            return this;
        }

        /// <summary>
        /// Stop playing, remove from ActiveTweens
        /// </summary>
        public void Stop(bool ignoreFlags = false)
        {
            if (!IsAlive) throw new Exception("Tween was killed before it could be stopped");
            if (!IsRunning) return;

            if (!ignoreFlags && _flags.KillWhenStopped)
            {
                Kill(ignoreFlags: false);
                return;
            }
            
            IsRunning = false;
            OnStop?.Invoke();
            ITweenManager.StopTween(this);
            
            if (!ignoreFlags && _flags.RestartWhenStopped)
            {
                Restart();
            }
        }

        /// <summary>
        /// Pause the Tween for a specified length, calls Stop if delay is 0f
        /// </summary>
        public void Pause(float timeToPause = 0f, Action onUnpause = null)
        {
            if (!IsAlive) throw new Exception("Tween was killed before it could be paused");
            if (IsPaused || !IsRunning) return;
            
            if (timeToPause == 0f)
            {
                Stop();
                return;
            }
            
            float pauseDelayElapsed = 0f;
            _pausePredicate = () =>
            {
                float deltaTime = GetDeltaTime;
                if (pauseDelayElapsed < timeToPause)
                {
                    pauseDelayElapsed += deltaTime;
                    return false;
                }
                return true;
            };
            
            IsPaused = true;
            _onUnpause = onUnpause;
        }

        /// <summary>
        /// Pause a Tween until a condition has been met
        /// </summary>
        public void Pause([NotNull] Func<bool> waitUntil, Action onUnpause = null)
        {
            if (!IsAlive) throw new Exception("Tween was killed before it could be paused");
            if (IsPaused || !IsRunning) return;
            
            _pausePredicate = waitUntil;
            
            IsPaused = true;
            _onUnpause = onUnpause;
        }

        //===== Lifetime =====
        
        /// <summary>
        /// Restart the Tween so it is ready to go again when called
        /// </summary>
        public void Restart(bool ignoreFlags = false)
        {
            _elapsed = 0f;
            _delayElapsed = 0f;
            _loopDelayElapsed = _settings.HangTime;
            _pausePredicate = null;

            _isForwards = _isStartingDirectionForwards;
            _activeEasingFunction = GetEasingFunction();

            _remainingLoops = _settings.LoopCount;
            
            Stop();

            if (!ignoreFlags)
            {
                if (_flags.RestorePosWhenRestart) _onUpdate.Invoke(0f);
                if (_flags.StartAutomatically) Start();
            }
        }

        /// <summary>
        /// Force the Tween to instantly complete
        /// </summary>
        public static void IT_ForceComplete(Tween tween, bool ignoreCompletion = false)
        {
            tween?.ForceCompletion(ignoreCompletion);
        }

        private void ForceCompletion(bool ignoreCompletion)
        {
            _onUpdate.Invoke(1f);
            if (!ignoreCompletion) OnComplete?.Invoke();
            Restart();
        }
        
        /// <summary>
        /// Force the Tween to return to starting position
        /// </summary>
        public static void IT_ForceReturn(Tween tween)
        {
            tween?.ForceReturn();
        }

        private void ForceReturn()
        {
            _onUpdate.Invoke(0f);
        }
        
        /// <summary>
        /// Destroy Tween
        /// </summary>
        public static void IT_Kill(Tween tween, bool ignoreFlags = false)
        {
            tween.Kill(ignoreFlags);
        }

        private void Kill(bool ignoreFlags)
        {
            if (!IsAlive) return;

            IsAlive = false;
            
            if (!ignoreFlags && _flags.CompleteWhenKilled) OnComplete?.Invoke();
            
            IsRunning = false;
            IsPaused = false;

            OnStart = null;
            OnStop = null;
            OnLoopComplete = null;
            OnComplete = null;
            
            _onUnpause = null;
            _pausePredicate = null;

            _target = null;
            
            ITweenManager.StopTween(this);
        }

        /// <summary>
        /// Reset a tween to its original state, can bring them back to life but callback events are lost
        /// </summary>
        public static Tween IT_Reset(Tween tween,
            [NotNull] UnityEngine.Object target, 
            Action onStart = null,
            Action onStop = null,
            Action onLoopComplete = null,
            Action onComplete = null)
        {
            return tween.Reset(target, onStart, onStop, onLoopComplete, onComplete);
        }

        private Tween Reset([NotNull] UnityEngine.Object target, 
            Action onStart = null,
            Action onStop = null,
            Action onLoopComplete = null,
            Action onComplete = null)
        {
            if (IsAlive && IsRunning) Kill(ignoreFlags: true);

            IsAlive = true;
            IsRunning = false;
            IsPaused = false;
            
            _target = target;

            OnStart += onStart;
            OnStop += onStop;
            OnLoopComplete += onLoopComplete;
            OnComplete += onComplete;
            _onUnpause = null;

            _elapsed = 0f;
            _delayElapsed = 0f;
            _loopDelayElapsed = _settings.HangTime;
            _pausePredicate = null;

            _isForwards = _isStartingDirectionForwards;
            _activeEasingFunction = GetEasingFunction();
            
            _remainingLoops = _settings.LoopCount;
            
            if (_flags.StartAutomatically) Start();
            return this;
        }
        
        //===== Helpers =====
        
        /// <summary>
        /// Create a new Inverted Tween based on the original
        /// </summary>
        public Tween Invert()
        {
            Tween inverted = new Tween(this);
            applyInvertedSettings(inverted);
            return inverted;

            static void applyInvertedSettings(Tween t)
            {
                t._isInverted = !t._isInverted;
                t._isForwards = !t._isInverted;
                t._activeEasingFunction = t.GetEasingFunction();
                t._isStartingDirectionForwards = t._isForwards;
            }
        }

        /// <summary>
        /// Switch the direction of motion on self
        /// </summary>
        public void InvertSelf()
        {
            _isInverted = !_isInverted;
            _isForwards = !_isInverted;
            _activeEasingFunction = GetEasingFunction();
        }
        
        private float GetDeltaTime
        {
            get
            {
                float deltaTime = Time.deltaTime;
                if (_flags.IgnoreTimeScale) deltaTime = Time.unscaledDeltaTime;
                return deltaTime;
            }
        }

        public override string ToString()
        {
            return _settings.AsString();
        }

        private Func<float, float> GetEasingFunction()
        {
            return (!_isForwards && _flags.IgnoreInvertEasing ) ? _invertedEasingFunction : _easingFunction;
        }
    }
}