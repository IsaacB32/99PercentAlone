using UnityEngine;

public class GravityCube : GravitySource
{
    [SerializeField] private float _width = 1f;
    [SerializeField] private float _thickness = 1f;
    
    //===== Properties =====

    public override float Mass => _surfaceGravity * _size * _width / (SpaceUtils.GRAVITATIONAL_CONST);

    public override Vector3 VectorToCenter(Vector3 objectPosition)
    {
        return MathUtils.ClosestPointOnPlaneSegment(transform, _size / 2f, _width / 2f, objectPosition) - objectPosition;
    }

    public override float DistanceToSurface(Vector3 objectPosition)
    {
        return MathUtils.DistanceToSurface(objectPosition, transform.position, _thickness);
    }

    protected override void ValidateSize()
    {
        transform.localScale = new Vector3(_size, _thickness, _width);
    }

    protected override void DrawIndicators(Transform testObject)
    {
        Vector3 left = MathUtils.ClosestPointOnPlaneSegment(transform, _size / 2f, _width / 2f, testObject.position);
        Gizmos.DrawLine(left, testObject.position);
    }
}
