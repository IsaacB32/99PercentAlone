using NaughtyAttributes;
using UnityEngine;

namespace ITween.Animator
{
    public class CanvasGroupAlphaAnimator : UITweenAnimator
    {
        [SerializeField, Required("A canvas group is required")] private CanvasGroup _groupTarget;
        [SerializeField, Range(0f,1f)] private float _hiddenAlpha;
        [SerializeField] private TweenSettings_Visibility _settings;
        
        protected override VisibilityTween InitializeTween()
        {
            return _groupTarget.IT_Alpha(_hiddenAlpha, _settings);
        }
    }
}