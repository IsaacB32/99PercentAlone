using System;
using UnityEngine;

//=!= CURRENTLY UNUSED =!=
namespace ITween.Exceptions
{
    public enum ITweenErrorType
    {
        KilledBefore,
        ApplySettingsFailed,
        VisibilityNotFresh,
        FlagMismatch,
        PathSize
    }
    
    /// <summary>
    /// Static class for Exception handling so they all come from the same place 
    /// </summary>
    public static class ITween_Errors
    {
        private static string KilledBefore(string beforeWhat) => $"Tween was killed before it could be {beforeWhat}";
        private static string ApplySettingsFailedMessage => "Tween cannot be updated after it has started, call Reset() first if Callbacks can be lost";
        private static string VisibilityNotFresh(string name) => $"{name} is not fresh {ApplySettingsFailedMessage}";
        private static string PathNotSizedCorrectly(string amount) => $"points must be longer than length 1, currently {amount}";
    
        public static void ThrowError(ITweenErrorType errorType, string msg)
        {
            TweenException loggedException = errorType switch
            {
                ITweenErrorType.KilledBefore => new KilledBeforeException(KilledBefore(msg)),
                ITweenErrorType.ApplySettingsFailed => new ApplySettingsFailed(ApplySettingsFailedMessage),
                ITweenErrorType.VisibilityNotFresh => new VisibilityNotFresh(VisibilityNotFresh(msg)),
                ITweenErrorType.FlagMismatch => new FlagMismatch(msg),
                ITweenErrorType.PathSize => new PathSize(PathNotSizedCorrectly(msg)),
                _ => throw new ArgumentOutOfRangeException(nameof(errorType), errorType, null)
            };
    
            if (Application.isEditor) throw loggedException;
            Debug.LogException(loggedException);
        }
    }
    
    public class TweenException : Exception { public TweenException(string msg) : base(msg) {} }
    public class KilledBeforeException : TweenException { public KilledBeforeException(string msg) : base(msg) {} }
    public class ApplySettingsFailed : TweenException { public ApplySettingsFailed(string msg) : base(msg) {} }
    public class VisibilityNotFresh : TweenException { public VisibilityNotFresh(string msg) : base(msg) {} }
    public class FlagMismatch : TweenException { public FlagMismatch(string msg) : base(msg) {} }
    public class PathSize : TweenException { public PathSize(string msg) : base(msg) {} }
}
