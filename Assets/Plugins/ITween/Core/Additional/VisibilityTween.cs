using System;
using JetBrains.Annotations;
using UnityEngine;

namespace ITween
{
    using Internal;
    
    /// <summary>
    /// Wrapper for Tweens, holds a visible and hidden Tween.
    /// Easy showing and hiding animations  
    /// </summary>
    public class VisibilityTween
    {
        //Core
        private UnityEngine.Object _target;
        private TweenSettings_Visibility _settings;
        private Tween _visibleTween, _hiddenTween;
        private Tween _activeTween;

        //Callbacks
        public event Action<bool> OnVisibilityChanged;
        private Action _onComplete;
        
        /// <summary>
        /// New Visibility Tween
        /// </summary>
        /// <param name="target">Object target</param>
        /// <param name="settings">Tween settings</param>
        /// <param name="visibleTween">visible tween</param>
        /// <param name="hiddenTween">hidden tween, leave blank for inverted hide</param>
        public VisibilityTween(
            [NotNull] UnityEngine.Object target,
            TweenSettings_Visibility settings,
            UnconfiguredTween visibleTween,
            UnconfiguredTween hiddenTween = null
        )
        {
            _target = target;
            _settings = settings;

            OnVisibilityChanged = null;

            _visibleTween = visibleTween.Configure(settings.VisibleSettings);
            _visibleTween.OnComplete += OnComplete;

            if (hiddenTween != null)
            {
                _hiddenTween = hiddenTween.Configure(getHiddenSettings());
            }
            else
            {
                _hiddenTween = visibleTween.Configure(getHiddenSettings()); 
                _hiddenTween.InvertSelf();
            }
            _hiddenTween.OnComplete += OnComplete;
            
            return;

            TweenSettings_Simple_Flagless getHiddenSettings()
            {
                return settings.UniqueSettingsOnHide ? settings.HiddenSettings : settings.VisibleSettings;
            }
        }

        private void OnComplete()
        {
            _onComplete?.Invoke();
            _onComplete = null;
            _activeTween = null;
        }

        public VisibilityTween SetVisible(bool visible, bool animate = true, Action onComplete = null)
        {
            OnVisibilityChanged?.Invoke(visible);

            _activeTween?.Restart();
            _activeTween = visible ? _visibleTween : _hiddenTween;
            
            if (!animate)
            {
                Tween.IT_ForceComplete(_activeTween, ignoreCompletion: true);
                OnComplete();
                return this;
            }
            
            _onComplete = onComplete;
            _activeTween.Start();
            return this;
        }
        
        //=!= Remove when it works correctly =!=
        //===== DEBUG METHOD =====

        public static void SetVisible(VisibilityTween vt)
        {
            Debug.Log("set visible");
            vt._visibleTween.Start();
        }

        public static void SetHidden(VisibilityTween vt)
        {
            Debug.Log("set hidden");
            vt._hiddenTween.Start();
        }
        
    }
}
