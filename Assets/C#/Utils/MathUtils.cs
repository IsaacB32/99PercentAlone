using UnityEngine;

public static class MathUtils
{
    /// <summary>
    ///  Returns the closest point on a line segment in world space 
    /// </summary>
    /// <param name="a">line point 1</param>
    /// <param name="b">line point 2</param>
    /// <param name="p">object point</param>
    /// <returns>closest point in line segment</returns>
    public static Vector3 ClosestPointOnLine(Vector3 a, Vector3 b, Vector3 p)
    {
        Vector3 ab = b - a;
        float abLengthSquared = Vector3.Dot(ab, ab);
        float t = Vector3.Dot(p - a, ab) / abLengthSquared;
        t = Mathf.Clamp01(t);
        return a + t * ab;
    }
    
    /// <summary>
    /// Returns the distance from P to the closest point on segment AB.
    /// </summary>
    public static float DistanceToSegment(Vector3 a, Vector3 b, Vector3 p)
    {
        Vector3 closest = ClosestPointOnLine(a, b, p);
        return Vector3.Distance(p, closest);
    }
    
    /// <summary>
    /// Returns the closest point on a bounded rectangular plane to point P.
    /// The plane is defined by its center, two perpendicular unit axis vectors
    /// (right and up), and half-extents along each axis.
    /// </summary>
    public static Vector3 ClosestPointOnPlaneSegment(Transform objectTransform, float half, Vector3 p)
    {
        Vector3 cp = p - objectTransform.position;

        // right/up are assumed unit length, so the dot product gives the
        // signed distance along each axis directly (no division needed).
        float u = Vector3.Dot(cp, objectTransform.right);
        float v = Vector3.Dot(cp, objectTransform.forward);

        // Clamp to +/- half-extent since the origin is the rectangle's center,
        // not a corner. This is the centered equivalent of the 0-1 clamp.
        u = Mathf.Clamp(u, -half, half);
        v = Mathf.Clamp(v, -half, half);

        return objectTransform.position + u * objectTransform.right + v * objectTransform.forward;
    }

    public static float DistanceToPlaneSegment(Transform objectTransform, float half, Vector3 p)
    {
        Vector3 closest = ClosestPointOnPlaneSegment(objectTransform, half, p);
        return Vector3.Distance(p, closest);
    }
}
