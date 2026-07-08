using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GravityHost : MonoBehaviour
{
    [SerializeField] private GravityType _gravityType;
    [Space]
    [SerializeField] private float _radius;
    [SerializeField] [Tooltip("for plane and cylinder types only")] private float _width;
    [SerializeField] private float _surfaceGravity;

    [Space]
    [SerializeField] private Transform _testObject;
    
    public float Mass { get; private set; }
        
    //===== Cylinder Gravity =====
    private Vector3 _linePointR, _linePointQ;

    private void Awake()
    {
        CalculateCylinderGravity();
    }

    public Vector3 VectorFromCenter(Vector3 objectPosition)
    {
        switch (_gravityType)
        {
            case GravityType.Cylinder:
                Vector3 finalPoint = MathUtils.ClosestPointOnLine(_linePointQ, _linePointR, objectPosition);
                return finalPoint - objectPosition;
            case GravityType.Plane:
                return MathUtils.ClosestPointOnPlaneSegment(transform, _radius / 2, objectPosition) - objectPosition;
            case GravityType.Sphere:
                return transform.position - objectPosition;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void CalculateCylinderGravity()
    {
        Vector3 world = transform.rotation * Vector3.up;
        Vector3 offset = world * _width;
        _linePointQ = transform.position - offset;
        _linePointR = transform.position + offset;
    }

    private void OnValidate() 
    {
        Mass = _surfaceGravity * _radius * _radius / PlayerMovementController.GRAVITATIONAL_CONST;
        ScalePlanet();
        return;
        
        void ScalePlanet()
        {
            transform.localScale = _gravityType switch
            {
                GravityType.Sphere => new Vector3(_radius, _radius, _radius),
                GravityType.Cylinder or GravityType.Plane => new Vector3(_radius, _width, _radius),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 left, right;
        if (!_testObject) return;
        switch (_gravityType)
        {
            case GravityType.Sphere:
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, _testObject.position);
                break;
            case GravityType.Cylinder:
                // Gizmos.color = Color.red;
                Vector3 world = transform.rotation * Vector3.up;
                Vector3 offset = world * _width;
                left = transform.position - offset;
                right = transform.position + offset;
                // Gizmos.DrawLine(left, right);
            
                Gizmos.color = Color.blue;
                Vector3 closestPoint = MathUtils.ClosestPointOnLine(left, right, _testObject.position);
                Gizmos.DrawLine(_testObject.transform.position, closestPoint);
                break;
            case GravityType.Plane:
                Gizmos.color = Color.blue;
                left = MathUtils.ClosestPointOnPlaneSegment(transform, _radius / 2, _testObject.position);
                Gizmos.DrawLine(left, _testObject.position);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        #if UNITY_EDITOR
        Handles.Label(transform.position + transform.up * 5f, $"Object Mass : {Mass}");
        #endif
    }

    public enum GravityType
    {
        Sphere, 
        Cylinder,
        Plane
    }
}


