using System;
using UnityEngine;

public class GravityCylinder : GravitySource
{
    [Header("Cylinder Settings")]
    [SerializeField] private float _width;

    public override float Size => Mathf.Max(_width, _size);

    private Vector3 _linePointQ;
    private Vector3 _linePointR;

    //===== Cylinder Settings
    private void Awake()
    {
        CalculateCylinderGravity();
    }

    private void CalculateCylinderGravity()
    {
        Vector3 world = transform.rotation * Vector3.up;
        Vector3 offset = world * (_width - _size / 2f);
        _linePointQ = transform.position - offset;
        _linePointR = transform.position + offset;
    }
    
    public override Vector3 VectorToCenter(Vector3 objectPosition)
    {
        Vector3 finalPoint = MathUtils.ClosestPointOnLine(_linePointQ, _linePointR, objectPosition);
        return finalPoint - objectPosition;
    }

    public override float DistanceToSurface(Vector3 objectPosition)
    {
        return MathUtils.DistanceToSurface(objectPosition, transform.position, _size);
    }

    protected override void ValidateSize()
    {
        transform.localScale = new Vector3(_size, _width, _size);
    }

    protected override void DrawIndicators(Transform testObject)
    {
        Vector3 world = transform.rotation * Vector3.up;
        Vector3 offset = world * (_width - _size / 2f);
        Vector3 left = transform.position - offset;
        Vector3 right = transform.position + offset;
        
        Vector3 closestPoint = MathUtils.ClosestPointOnLine(left, right, testObject.position);
        Gizmos.DrawLine(testObject.transform.position, closestPoint);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(left, right);
    }
}
