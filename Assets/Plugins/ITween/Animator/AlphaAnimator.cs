using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace ITween.Animator
{
    using Internal;
    
    public class AlphaAnimator : TweenAnimator
    {
        [SerializeField, Required("A graphic is required")] private Graphic _graphicTarget;
        [SerializeField, Range(0f,1f)] private float _hiddenAlpha;
        [SerializeField] private TweenSettings_Visibility _settings;

        protected override VisibilityTween InitializeTween()
        {
            float fromAlpha = _graphicTarget.color.a;
            
            UnconfiguredTween visible = ITManager.IT_Value(_graphicTarget, 0f, 1f,
                t =>
                {
                    Color tmp = _graphicTarget.color;
                    tmp.a = Mathf.LerpUnclamped(_hiddenAlpha, fromAlpha, t);
                    _graphicTarget.color = tmp;
                }
            );
            
            UnconfiguredTween hidden = ITManager.IT_Value(_graphicTarget, 0f, 1f,
                t =>
                {
                    Color tmp = _graphicTarget.color;
                    tmp.a = Mathf.LerpUnclamped(fromAlpha, _hiddenAlpha, t);
                    _graphicTarget.color = tmp;
                }
            );

            return new VisibilityTween(_graphicTarget, _settings, visible, hidden);
        }
        
        protected override void AssignType() => ImplicitAssignType(ref _graphicTarget, gameObject);
    }
}
