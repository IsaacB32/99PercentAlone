using UnityEngine;

namespace ITween.Internal
{
    public static class TransformExtensions
    {
        public static TransformSnapshot ToSnapshot(this Transform t, bool isLocal)
        {
            return new TransformSnapshot(t, isLocal);
        }
    }
    
    public struct TransformSnapshot
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public bool IsValid { get; private set; }
        
        public TransformSnapshot(Transform t, bool isLocal)
        {
            position = !isLocal ? t.position : t.localPosition;
            rotation = !isLocal ? t.rotation : t.localRotation;
            scale = t.localScale;
            IsValid = true;
        }
        
        public void ApplyTo(Transform t)
        {
            t.position = position;
            t.rotation = rotation;
            t.localScale = scale;
        }
    }
}