using UnityEngine;

public static class SpaceUtils
{
    public const float GRAVITATIONAL_CONST = 0.01f;
    
    /// <summary>
    /// A GravitySource object relative to another object
    /// </summary>
    public struct RelativeGravitySource
    {
        public GravitySource gravitySource;
        public Vector3 strongestGravitationalPull;
    }
}
