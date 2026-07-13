using System;
using System.Linq;
using UnityEngine;

public enum GravityBodyType
{
    Source,
    Directional,
    Reassign
}

/// <summary>
/// Physics objects that are effected by the gravity system
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
    [Header("Gravity Settings")]
    [SerializeField] protected float _weakestGravityStrength = 1f;
    [Tooltip("the max speed the parent object slerps at when changing movement states")] 
    [SerializeField] protected float _gravitySmoothingMax = 5f;
    [SerializeField] private float _directionalAcceleration = -9.8f;
    
    [Header("Static Gravity Sources")]
    [Tooltip("GravitySources that are always calculated")]
    [SerializeField] protected GravitySource[] _staticGravitySources;
    
    //===== References =====
    protected Rigidbody _rb { get; private set; }
    
    //===== Gravity =====
    private Action CurrentGravitySystem;
    private GravitySource[] _sources = Array.Empty<GravitySource>();
    public Vector3 DirectionalGravity { get; set; } = Vector3.zero;
    
    private GravityBodyType _gravityType = GravityBodyType.Source; 
    public GravityBodyType GravityType
    {
        get => _gravityType;
        set
        {
            switch (value)
            {
                case GravityBodyType.Directional when DirectionalGravity == Vector3.zero:
                    throw new Exception("Directional gravity selected but vector = zero");
                case GravityBodyType.Reassign:
                    FindEffectors();
                    break;
                case GravityBodyType.Source:
                    CurrentGravitySystem = ApplySourceGravity;
                    break;
                case GravityBodyType.Directional:
                    CurrentGravitySystem = ApplyDirectionalGravity;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }

            _gravityType = value;
        }
    }
    
    protected void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        GravityType = _gravityType;
        FindEffectors();
    } 
    
    //===== Gravity Applications =====
    
    protected void FixedUpdate()
    {
        CurrentGravitySystem();
    }
    
    protected virtual void ApplySourceGravity()
    {
        GetClosestSourceToObject(out Vector3 pull, out float? distanceToSurface);
        RotateObjectToSourceUp(pull, distanceToSurface);
    }

    protected virtual void ApplyDirectionalGravity()
    {
        Vector3 acceleration = DirectionalGravity * _directionalAcceleration;
        _rb.AddForce(acceleration, ForceMode.Acceleration);
        
        Vector3 gravityUp = DirectionalGravity.normalized;
        Quaternion deltaRotation = Quaternion.FromToRotation(transform.up, gravityUp);
        Quaternion targetRotation = deltaRotation * _rb.rotation;
        
        Quaternion easedRot = Quaternion.Slerp(_rb.rotation, targetRotation, _gravitySmoothingMax * Time.fixedDeltaTime);
        _rb.rotation = easedRot;
        
    }

    #region Gravity Calculations

    /// <summary>
    /// Apply all source gravity to the object and return the closest source
    /// </summary>
    /// <param name="pull">the pull of the closest source</param>
    /// <param name="distanceToSurface">distance from the object to the source surface</param>
    protected GravitySource GetClosestSourceToObject(out Vector3 pull, out float? distanceToSurface)
    {
        GravitySource closestSource = null;
        pull = Vector3.zero;
        foreach (GravitySource source in _sources)
        {
            Vector3 vectorToCenter = source.VectorToCenter(_rb.position);
            float sqrDst = vectorToCenter.sqrMagnitude;
            Vector3 forceDir = vectorToCenter.normalized;
            Vector3 acceleration = forceDir * SpaceUtils.GRAVITATIONAL_CONST * source.Mass / sqrDst;
            
            _rb.AddForce(acceleration, ForceMode.Acceleration);
        
            if (acceleration.sqrMagnitude > pull.sqrMagnitude)
            {
                pull = acceleration;
                closestSource = source;
            }
        }

        distanceToSurface = closestSource?.DistanceToSurface(_rb.position);
        return closestSource;
    }

    /// <summary>
    /// Rotate the object to the up direction of a given source
    /// </summary>
    /// <param name="pull">pull of the source object</param>
    /// <param name="distanceToSurface">distance to the surface of the source</param>
    protected void RotateObjectToSourceUp(Vector3 pull, float? distanceToSurface)
    {
        if (!distanceToSurface.HasValue) return;
        if (!(pull.sqrMagnitude >= _weakestGravityStrength)) return;
        
        Vector3 gravityUp = -pull.normalized;
        
        Quaternion deltaRotation = Quaternion.FromToRotation(transform.up, gravityUp);
        Quaternion targetRotation = deltaRotation * _rb.rotation;
        
        float cameraSmoothSpeed = _gravitySmoothingMax / (1f + distanceToSurface.Value * 0.1f);
        Quaternion easedRot = Quaternion.Slerp(_rb.rotation, targetRotation, cameraSmoothSpeed * Time.fixedDeltaTime);
        _rb.rotation = easedRot;
    }

    #endregion
    
    //===== Gravity Triggers =====
    
    public void UpdateGravitySources(GravitySource[] sources)
    {
        _sources = _staticGravitySources.Concat(sources).ToArray();
    }

    public void RemoveGravitySources()
    {
        UpdateGravitySources(Array.Empty<GravitySource>());
    }
    
    /// <summary>
    /// A unit sphere to check if started inside a GravityEventTrigger
    /// </summary>
    private void FindEffectors()
    {
        bool hasDirectionalTrigger = false;
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f, Layers.ToLayerMask(Layers.GravityTrigger));
        foreach (Collider hit in hits)   
        {
            if (hit.isTrigger)
            {
                GravityEventTrigger trigger = hit.GetComponent<GravityEventTrigger>();
                trigger.OnGravityBodyEnter(this);

                if (hasDirectionalTrigger) Debug.LogWarning("Two directional triggers found, possible errors!");
                hasDirectionalTrigger = hasDirectionalTrigger || trigger is GravityDirectionalTrigger;
            }
        }
        
        GravityType = hasDirectionalTrigger ? GravityBodyType.Directional : GravityBodyType.Source;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.aquamarine;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
