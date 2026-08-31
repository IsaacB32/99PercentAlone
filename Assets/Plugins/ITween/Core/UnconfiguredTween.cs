using System;
using JetBrains.Annotations;

namespace ITween.Internal
{
    /// <summary>
    /// To be used when a Tween is needed but has no settings 
    /// </summary>
    public class UnconfiguredTween
    {
        private UnityEngine.Object _target;
        private event Action<float> _onUpdate;
        
        public UnconfiguredTween(
            [NotNull] UnityEngine.Object target, 
            [NotNull] Action<float> onUpdate
        )
        {
            _target = target;
            _onUpdate = onUpdate;
        }

        /// <summary>
        /// Turn the Unconfigured Tween into a usable Tween
        /// </summary>
        public Tween Configure([NotNull] ITweenSettings settings)
        {
            if (_onUpdate == null) throw new Exception("_onUpdate cannot be null");
            Tween t = new Tween(_target, settings, _onUpdate);
            return t;
        }
    }
}
