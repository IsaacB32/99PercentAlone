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
    
    /// <summary>
    /// Apply all source gravity to object and return the closest relative source
    /// </summary>
    /// <param name="gravityObject">reference object</param>
    // public static RelativeGravitySource GetClosestSourceToObject(GravityBody gravityObject)
    // {
    //     RelativeGravitySource relativeSource;
    //     relativeSource.gravitySource = null;
    //     relativeSource.strongestGravitationalPull = Vector3.zero;
    //     
    //     foreach (GravitySource body in gravityObject.Sources)
    //     {
    //         Vector3 vectorToCenter = body.VectorToCenter(gravityObject.rb.position);
    //         float sqrDst = vectorToCenter.sqrMagnitude;
    //         Vector3 forceDir = vectorToCenter.normalized;
    //         Vector3 acceleration = forceDir * GRAVITATIONAL_CONST * body.Mass / sqrDst;
    //         
    //         gravityObject.rb.AddForce(acceleration, ForceMode.Acceleration);
    //     
    //         if (acceleration.sqrMagnitude > relativeSource.strongestGravitationalPull.sqrMagnitude)
    //         {
    //             relativeSource.strongestGravitationalPull = acceleration;
    //             relativeSource.gravitySource = body;
    //         }
    //     }
    //
    //     return relativeSource;
    // }
    
    /// <summary>
    /// Rotate an object to the up direction of a given source
    /// </summary>
    /// <param name="gravityObject">reference object</param>
    /// <param name="relativeSource">source object</param>
    // public static void RotateObjectToSourceUp(GravityBody gravityObject, RelativeGravitySource relativeSource)
    // {
    //     if (!relativeSource.gravitySource) return;
    //     if (!(relativeSource.strongestGravitationalPull.sqrMagnitude >= gravityObject.WeakestGravityStrength)) return;
    //     
    //     Vector3 gravityUp = -relativeSource.strongestGravitationalPull.normalized;
    //     
    //     Quaternion deltaRotation = Quaternion.FromToRotation(gravityObject.transform.up, gravityUp);
    //     Quaternion targetRotation = deltaRotation * gravityObject.rb.rotation;
    //
    //     float distanceToSurface = relativeSource.gravitySource.DistanceToSurface(gravityObject.rb.position);
    //     float cameraSmoothSpeed = gravityObject.GravitySmoothingMax / (1f + distanceToSurface * 0.1f);
    //     Quaternion easedRot = Quaternion.Slerp(gravityObject.rb.rotation, targetRotation, cameraSmoothSpeed * Time.fixedDeltaTime);
    //     gravityObject.rb.rotation = easedRot;
    // }
}
