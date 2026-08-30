using UnityEngine;

namespace ITween.Internal
{
    public enum LoopType
    {
        Single,    //play once
        PingPong,  //there and back
        Repeat     //loop from beginning
    }
    
    /// <summary>
    /// Boolean flags for special cases for Tweens
    /// </summary>
    [System.Serializable]
    public class TweenFlags
    {
        [SerializeField] [Tooltip("Animates with UnscaledTimeScale")]
        private bool _ignoreTimeScale = false;
        public bool IgnoreTimeScale => _ignoreTimeScale;

        [SerializeField] [Tooltip("Easing Curve Inverted on return")]
        private bool _ignoreInvertEasing = false;
        public bool IgnoreInvertEasing => _ignoreInvertEasing;

        [SerializeField] [Tooltip("Start at End, End at Start")]
        private bool _invertStartingDirection = false;
        public bool InvertStartingDirection => _invertStartingDirection;

        [SerializeField] [Tooltip("Start animation when created")]
        private bool _startAutomatically = false;
        public bool StartAutomatically => _startAutomatically;

        [SerializeField] [Tooltip("Restart once stopped")]
        private bool _restartWhenStopped = false;
        public bool RestartWhenStopped => _restartWhenStopped;

        [SerializeField] [Tooltip("Fire onComplete action when killed")]
        private bool _completeWhenKilled = false;
        public bool CompleteWhenKilled => _completeWhenKilled;

        [SerializeField] [Tooltip("Kill once stopped rendering unusable until Restart()")]
        private bool _killWhenStopped = false;
        public bool KillWhenStopped => _killWhenStopped;

        [SerializeField] [Tooltip("Return position back to original when Restart()")]
        private bool _restorePosWhenRestart = false;
        public bool RestorePosWhenRestart => _restorePosWhenRestart;

        public TweenFlags(
            bool ignoreTimeScale = false,
            bool ignoreInvertEasing = false,
            bool invertStartingDirection = false,
            bool startAutomatically = false,
            bool restartWhenStopped = false,
            bool completeWhenKilled = false,
            bool killWhenStopped = false,
            bool restorePosWhenRestart = false
        )
        {
            _ignoreTimeScale = ignoreTimeScale;
            _ignoreInvertEasing = ignoreInvertEasing;
            _invertStartingDirection = invertStartingDirection;
            _startAutomatically = startAutomatically;
            _restartWhenStopped = restartWhenStopped;
            _completeWhenKilled = completeWhenKilled;
            _killWhenStopped = killWhenStopped;
            _restorePosWhenRestart = restorePosWhenRestart;
        }
    }
    
    [System.Serializable]
    public class TweenFlags_Visibility
    {
        [SerializeField] [Tooltip("Animates with UnscaledTimeScale")] private bool _ignoreTimeScale = false;
        [SerializeField] [Tooltip("Easing Curve Inverted on return")] private bool _ignoreInvertEasing = false;
        [SerializeField] [Tooltip("Start at End, End at Start")] private bool _invertStartingDirection = false;

        public TweenFlags_Visibility(
            bool ignoreTimeScale = false,
            bool ignoreInvertEasing = false,
            bool invertStartingDirection = false
        )
        {
            _ignoreTimeScale = ignoreTimeScale;
            _ignoreInvertEasing = ignoreInvertEasing;
            _invertStartingDirection = invertStartingDirection;
        }

        public static implicit operator TweenFlags(TweenFlags_Visibility flags) => 
            new TweenFlags(flags._ignoreTimeScale, flags._ignoreInvertEasing, flags._invertStartingDirection);

        public override string ToString()
        {
            return $"flags : \n" +
                   $"IgnoreTimeScale: {_ignoreTimeScale}\n" +
                   $"IgnoreInvertEasing: {_ignoreInvertEasing}\n" +
                   $"InvertStartingDirection: {_invertStartingDirection}";
        }
    }
}
