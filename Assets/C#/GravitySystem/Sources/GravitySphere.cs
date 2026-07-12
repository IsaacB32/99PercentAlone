using UnityEngine;

public class GravitySphere : GravitySource
{
    public override Vector3 VectorToCenter(Vector3 objectPosition)
    {
        return transform.position - objectPosition;
    }

    public override float DistanceToSurface(Vector3 objectPosition)
    {
        return MathUtils.DistanceToSurface(objectPosition, transform.position, _size / 2f);
    }

    protected override void ValidateSize()
    {
        transform.localScale = Vector3.one * _size;
    }
    
    protected override void DrawIndicators(Transform testObject)
    {
        Gizmos.DrawLine(transform.position, testObject.position);
    }
}
