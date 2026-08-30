using NaughtyAttributes;
using UnityEngine;

namespace ITween
{
    using Internal;
    
    #region Tween Settings

    /// <summary>
    /// A version of Simple TweenSettings without Flags, for easy code-configuring
    /// </summary>
    [System.Serializable]
    public class TweenSettings_Simple_Flagless : ITweenSettings
    {
        //===== Duration =====
        [field: SerializeField, Min(0.1f)] public float Duration { get; private set; }
        
        //===== Easing =====
        [field: SerializeField] public EasingType EaseType { get; private set; }
        
        [SerializeField, AllowNesting, ShowIf(nameof(ShowOvershoot))] private float _overshoot; 
        public float Overshoot => _overshoot;
        
        [SerializeField]
        [CurveRange(0, 0, 1, 1, EColor.Red), ShowIf(nameof(this.EaseType), enumValue: EasingType.Custom)] private AnimationCurve _customCurve;
        public AnimationCurve CustomCurve => _customCurve;
        
        public TweenFlags Flags { get; private set; }
        public float DelayTime { get; set; } = 0f;
        public LoopType LoopingType { get; set; } = LoopType.Single;
        public int LoopCount { get; set; } = 1;
        public float HangTime { get; set; } = 0f;

        public TweenSettings_Simple_Flagless(
            float duration, 
            EasingType easeType,
            float overshoot, 
            AnimationCurve customCurve)
        {
            Duration = duration;
            EaseType = easeType;
            _overshoot = overshoot;
            _customCurve = customCurve;

            Flags = new TweenFlags();
        }
        
        public TweenSettings_Simple_Flagless()
        {
            Duration = 0.7f;
            EaseType = EasingType.OutBack;
            _overshoot = ITweenSettings.OVERSHOOT_AMOUNT;

            Flags = new TweenFlags();
        }
        
        //===== Helpers =====

        public void ApplyFlags(TweenFlags flags)
        {
            Flags = flags;
        }
        
        /// <summary>
        /// instance method to check if overshoot should be shown, used by NaughtyAttributes 
        /// </summary>
        private bool ShowOvershoot()
        {
            return EaseType switch
            {
                EasingType.InBack or EasingType.OutBack or EasingType.InOutBack => true,
                _ => false
            };
        }
    }
    
    /// <summary>
    /// A simple version of TweenSettings, used for quick or non-complex Tweening
    /// </summary>
    [System.Serializable]
    public class TweenSettings_Simple: ITweenSettings
    {
        //===== Duration =====
        [field: SerializeField, Min(0.1f)] public float Duration { get; private set; }
        
        //===== Easing =====
        [field: SerializeField] public EasingType EaseType { get; private set; }
        
        [SerializeField, AllowNesting, ShowIf(nameof(ShowOvershoot))] private float _overshoot; 
        public float Overshoot => _overshoot;
        
        [SerializeField]
        [CurveRange(0, 0, 1, 1, EColor.Red), ShowIf(nameof(this.EaseType), enumValue: EasingType.Custom)] private AnimationCurve _customCurve;
        public AnimationCurve CustomCurve => _customCurve;

        //===== Flags =====
        [field: SerializeField] public TweenFlags Flags { get; private set; }

        public float DelayTime => 0f;
        public LoopType LoopingType => LoopType.Single;
        public int LoopCount => 1;
        public float HangTime => 0f;

        public TweenSettings_Simple(
            float duration, 
            EasingType easeType,
            float overshoot, 
            TweenFlags flags,
            AnimationCurve customCurve)
        {
            Duration = duration;
            EaseType = easeType;
            _overshoot = overshoot;
            _customCurve = customCurve;
            Flags = flags;
        }

        public TweenSettings_Simple(float duration)
        {
            Duration = duration;
            EaseType = EasingType.OutBack;
            _overshoot = ITweenSettings.OVERSHOOT_AMOUNT;
            Flags = new TweenFlags();
        }

        public TweenSettings_Simple()
        {
            Duration = 0.7f;
            EaseType = EasingType.OutBack;
            _overshoot = ITweenSettings.OVERSHOOT_AMOUNT;
            Flags = new TweenFlags();
        }
        
        //===== Helpers =====
        
        /// <summary>
        /// instance method to check if overshoot should be shown, used by NaughtyAttributes 
        /// </summary>
        private bool ShowOvershoot()
        {
            return EaseType switch
            {
                EasingType.InBack or EasingType.OutBack or EasingType.InOutBack => true,
                _ => false
            };
        }
    }

    /// <summary>
    /// All the settings needed for a basic Tween animation 
    /// </summary>
    [System.Serializable]
    public class TweenSettings: ITweenSettings
    {
        //===== Duration =====
        [SerializeField, Min(0.1f)] protected float _duration;
        public float Duration => _duration;
        
        //===== Easing =====
        [SerializeField] protected EasingType _easeType;
        public EasingType EaseType => _easeType;
        
        [SerializeField, AllowNesting, ShowIf(nameof(ShowOvershoot))] protected float _overshoot;
        public float Overshoot => _overshoot;
        
        [SerializeField]
        [CurveRange(0, 0, 1, 1, EColor.Red), ShowIf(nameof(this.EaseType), enumValue: EasingType.Custom)] protected AnimationCurve _customCurve;
        public AnimationCurve CustomCurve => _customCurve;
        
        //===== Delay =====
        [SerializeField] protected float _delayTime;
        public float DelayTime => _delayTime;
 
        //===== Looping =====
        [SerializeField] protected LoopType _loopingType;
        public LoopType LoopingType => _loopingType;
        
        [SerializeField, AllowNesting, Min(1)] [HideIf(nameof(this.LoopingType), enumValue: LoopType.Single)] protected int _loopCount;
        [SerializeField, AllowNesting] [HideIf(nameof(this.LoopingType), enumValue: LoopType.Single)] protected float _hangTime;
        public float HangTime => _hangTime;
        public int LoopCount => _loopCount;
        
        //===== Flags =====
        [SerializeField] protected TweenFlags _flags;
        public TweenFlags Flags => _flags;
        
        //===== Constructors =====

        public TweenSettings(
            float duration,
            EasingType easeType = EasingType.OutBack,
            float overshoot = ITweenSettings.OVERSHOOT_AMOUNT,
            AnimationCurve customCurve = null,
            float delayTime = 0f,
            LoopType loopingType = LoopType.Single,
            int loopCount = 1,
            float hangTime = 0,
            TweenFlags flags = null
        ) 
        {
            _duration = duration;
            _easeType = easeType;
            _overshoot = overshoot;
            _customCurve = customCurve;
            
            _delayTime = delayTime;
            _loopingType = loopingType;
            _loopCount = loopCount;
            _hangTime = hangTime;

            _flags = flags ?? new TweenFlags();
        }

        public TweenSettings()
        {
            _duration = 0.7f;
            _easeType = EasingType.OutBack;
            _overshoot = ITweenSettings.OVERSHOOT_AMOUNT;
            
            _delayTime = 0f;
            _loopingType = LoopType.Single;
            _loopCount = 1;
            _hangTime = 0f;
            
            _flags = new TweenFlags();
        }
        
        /// <summary>
        /// instance method to check if overshoot should be shown, used by NaughtyAttributes 
        /// </summary>
        private bool ShowOvershoot()
        {
            return _easeType switch
            {
                EasingType.InBack or EasingType.OutBack or EasingType.InOutBack => true,
                _ => false
            };
        }

        //===== Prebuilt Settings =====

        public static ITweenSettings EmptySettings = new TweenSettings_Empty();
        private class TweenSettings_Empty : ITweenSettings
        {
            public float Duration => 0f;
            public EasingType EaseType => EasingType.OutCubic;
            public float Overshoot => 0f;
            public AnimationCurve CustomCurve => null;
            public TweenFlags Flags => new TweenFlags();
            public float DelayTime => 0f;
            public LoopType LoopingType => LoopType.Single;
            public int LoopCount => 0;
            public float HangTime => 0f;
        }
    }
    
    #endregion
    
    #region Path Settings
    
    /// <summary>
    /// Settings for configuring Path Tweens
    /// </summary>
    [System.Serializable]
    public class TweenSettings_Path : TweenSettings
    {
        [Header("Path")]
        [SerializeField, ValidateInput(nameof(ValidatePathType), "transform type must have value")]
        [EnumFlags] private PathTransformType _pathTransformType;
        public PathTransformType TransformType => _pathTransformType;
        
        public TweenSettings_Path()
        {
            _overshoot = 0f;
            _easeType = EasingType.OutCubic;

            _pathTransformType = PathTransformType.Position;
        }
        
        [System.Flags]
        public enum PathTransformType
        {
            None = 0,
            Position = 1 << 0,
            Rotation = 1 << 1,
            Scale = 1 << 2
        }

        private bool ValidatePathType()
        {
            return _pathTransformType != PathTransformType.None;
        }
    }
    
    #endregion
    
    #region Visibility Settings
    
    /// <summary>
    /// Controls for Visibility Tweens
    /// </summary>
    [System.Serializable]
    public class TweenSettings_Visibility
    {
        public bool UniqueSettingsOnHide = false;
        
        [SerializeField] private TweenSettings_Simple_Flagless _visibleSettings;
        public TweenSettings_Simple_Flagless VisibleSettings
        {
            get
            {
                _visibleSettings.ApplyFlags(Flags);
                return _visibleSettings;
            }
        }

        [SerializeField, AllowNesting, ShowIf(nameof(UniqueSettingsOnHide))] private TweenSettings_Simple_Flagless _hiddenSettings;
        public TweenSettings_Simple_Flagless HiddenSettings
        {
            get
            { 
                _hiddenSettings.ApplyFlags(Flags);
                return _hiddenSettings;
            }
        }

        [field: SerializeField] public TweenFlags_Visibility Flags { get; private set; }
    }
    
    #endregion
}

namespace ITween.Internal
{
    public interface ITweenSettings
    {
        public const float OVERSHOOT_AMOUNT = 1.70158f;
        
        float Duration { get; }
        EasingType EaseType { get; }
        float Overshoot { get; }
        AnimationCurve CustomCurve { get; }
        TweenFlags Flags { get; }
        float DelayTime { get; }
        LoopType LoopingType { get; }
        int LoopCount { get; }
        float HangTime { get; }

        public string AsString()
        {
            return "TweenSettings: \n" +
                   $"Duration: {Duration}\n" +
                   $"EaseType: {EaseType}\n" +
                   $"Overshoot: {Overshoot}\n" +
                   $"Flags: {Flags}\n" +
                   $"DelayTime: {DelayTime}\n" +
                   $"LoopType: {LoopingType}\n" +
                   $"LoopCount: {LoopCount}\n" +
                   $"HangTime: {HangTime}";
        }
    }
}