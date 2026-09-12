using UnityEngine;

namespace Isaac.Extensions
{
    public static class FloatExtensions
    {
        /// <summary>
        /// Square a float
        /// </summary>
        public static float sqr(this float target) => target * target;
    }

    public static class VectorExtensions
    {
        public static Vector3 Rotated(this Vector3 vector, Quaternion rotation, Vector3 pivot = default)
        {
            return rotation * (vector - pivot) + pivot;
        }

        public static Vector3 Rotated(this Vector3 vector, Vector3 rotation, Vector3 pivot = default)
        {
            return Rotated(vector, Quaternion.Euler(rotation), pivot);
        }

        public static Vector3 Rotated(this Vector3 vector, float x, float y, float z, Vector3 pivot = default)
        {
            return Rotated(vector, Quaternion.Euler(x, y, z), pivot);
        }
    }

    public static class TransformExtensions
    {
        public static TransformSnapshot ToSnapshot(this Transform t)
        {
            return new TransformSnapshot(t);
        }
    }

    public struct TransformSnapshot
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public bool IsValid { get; private set; }
        
        public TransformSnapshot(Transform t)
        {
            position = t.position;
            rotation = t.rotation;
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

