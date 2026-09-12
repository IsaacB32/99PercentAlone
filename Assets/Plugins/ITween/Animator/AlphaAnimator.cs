using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace ITween.Animator
{
    public class AlphaAnimator : UITweenAnimator
    {
        [SerializeField, Required("A graphic is required")] private Graphic _graphicTarget;
        [SerializeField, Range(0f,1f)] private float _hiddenAlpha;
        [SerializeField] private TweenSettings_Visibility _settings;

        protected override VisibilityTween InitializeTween()
        {
            return _graphicTarget.IT_Alpha(_hiddenAlpha, _settings);
        }
    }
}
