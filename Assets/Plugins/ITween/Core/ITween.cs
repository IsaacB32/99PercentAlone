using System;
using System.Collections;

namespace ITween.Internal
{
    /// <summary>
    /// A basic group for all Tweens to share similar properties
    /// </summary>
    public interface ITween
    {
        //===== ID =====
        public int IDKey { get; }
        
        //===== Core Properties =====
        public bool IsAlive { get; }      //lifetime of the Tween
        public bool IsRunning { get; }    //is active
        public bool IsPaused { get; }     //is active under pause predicate
        
        //===== Lifetime =====
        
        public void Update();
        
        //===== Control =====

        public void Kill(bool ignoreFlags);
    }
}