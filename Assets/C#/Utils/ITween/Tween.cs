using System;
using UnityEngine;

namespace ITween
{
    public class Tween
    {
        public bool IsAlive { get; private set; } = true;
        private float _elapsed;

        private UnityEngine.Object _target;
        private float _duration;
        private event Action<float> _onUpdate;
        private event Action _onComplete;
        private Func<float, float> _easingFunction;

        public Tween(
            UnityEngine.Object target,
            float duration,
            Action<float> onUpdated,
            Action onComplete,
            Func<float, float> easingFunction
        )
        {
            _target = target;
            _duration = duration;
            _onUpdate = onUpdated;
            _onComplete = onComplete;
            _easingFunction = easingFunction;
        }

        public void Update(float deltaTime)
        {
            if (!this.IsAlive || _target == null && _target.Equals(null))
            {
                this.IsAlive = false;
                return;
            }
            
            _elapsed += deltaTime;
            float t = Mathf.Clamp01(_elapsed / _duration);
            float easedT = _easingFunction.Invoke(t);
            
            _onUpdate?.Invoke(easedT);

            if (t >= 1f)
            {
                this.IsAlive = false;
                _onComplete?.Invoke();
            }
        }

        public Tween SetEase(EasingType easingType)
        {
            _easingFunction = Easing.GetEasingFunction(easingType);
            return this;
        }

        public void Kill()
        {
            _target = null;
            this.IsAlive = false;
        }
    }
}
