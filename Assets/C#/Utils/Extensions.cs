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
        public static Vector3 Rotated(this Vector3 vector, Quaternion rotation, Vector3 pivot = default(Vector3)) {
            return rotation * (vector - pivot) + pivot;
        }

        public static Vector3 Rotated(this Vector3 vector, Vector3 rotation, Vector3 pivot = default(Vector3)) {
            return Rotated(vector, Quaternion.Euler(rotation), pivot);
        }

        public static Vector3 Rotated(this Vector3 vector, float x, float y, float z, Vector3 pivot = default(Vector3)) {
            return Rotated(vector, Quaternion.Euler(x, y, z), pivot);
        }
    }
}

