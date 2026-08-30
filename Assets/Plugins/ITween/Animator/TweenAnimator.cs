using UnityEngine;

namespace ITween.Animator
{
    /// <summary>
    /// Wrapper for Tweens to show Editor playback buttons
    /// </summary>
    public abstract class TweenAnimator : MonoBehaviour
    {
        public Tween ActiveTween { get; private set; }
        
        protected virtual void Awake()
        {
            ActiveTween = InitializeTween();
        }
        
        protected abstract Tween InitializeTween();
    }
    
    /// <summary>
    /// Wrapper for VisibilityTweens to show Editor playback buttons
    /// </summary>
    public abstract class VisibilityAnimator : MonoBehaviour
    {
        //TODO : write editor for this
        public VisibilityTween ActiveTween { get; private set; }
        
        protected virtual void Awake()
        {
            ActiveTween = InitializeTween();
        }
        
        protected abstract VisibilityTween InitializeTween();
    }
}

//=!= A failed idea for a generic implementation of Tween Animation =!=
/*
   /// <summary>
   /// MonoBehavior class for quickly adding Tween animations to objects
   /// </summary>
   public abstract class TweenAnimator<T, S> : TweenPlayer 
       where S : ITweenSettings 
   {
       [SerializeField] protected T _target;
       [SerializeField] protected S _settings;
   } 
*/
