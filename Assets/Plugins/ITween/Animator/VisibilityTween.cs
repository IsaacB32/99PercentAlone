using System;
using System.Collections;
using JetBrains.Annotations;

namespace ITween.Animator
{
    using Internal;
    
    /// <summary>
    /// Wrapper for Tweens, holds a visible and hidden Tween.
    /// Easy showing and hiding animations  
    /// </summary>
    public sealed class VisibilityTween
    {
        //Core
        private UnityEngine.Object _target;
        public TweenSettings_Visibility Settings { get; }
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
            Settings = settings;

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

        public IEnumerator AsCoroutine() => _activeTween?.AsCoroutine(); 
    }
}
