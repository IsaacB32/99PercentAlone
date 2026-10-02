using System;
using System.Collections;
using UnityEngine;

namespace ITween.Animator
{
    public abstract class TweenAnimator : MonoBehaviour
    {
        private VisibilityTween _activeHandle;

        protected void Awake()
        {
            if (_activeHandle != null) return;
            _activeHandle = InitializeTween();
        }
        
        /// <summary>
        /// Set the ActiveTween
        /// </summary>
        protected abstract VisibilityTween InitializeTween();

        protected static void ForceInitializeGroup(TweenAnimator[] animators)
        {
            foreach (TweenAnimator animator in animators)
            {
                animator._activeHandle ??= animator.InitializeTween();
            }
        }
        
        //===== Tween Calls =====
        
        public virtual VisibilityTween SetVisible(bool visible, bool animate = true, Action onComplete = null)
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (_activeHandle == null) throw new Exception("ActiveHandle was not initialized");
            return _activeHandle.SetVisible(visible, animate, onComplete);
        }
        
        //===== Implicit Typing =====

        /// <summary>
        /// Implicitly assign the _target in the inspector
        /// </summary>
        protected virtual void AssignType()
        {
            //override point
        }
        
        protected void Reset() => AssignType();
        
        protected static void ImplicitAssignType<TType>(ref TType component, GameObject o) where TType : Component
        {
            if (component != null) return;
            TType[] components = o.GetComponents<TType>();
            component = components.Length is 0 or > 1 ? null : components[0];
        }
    }
}