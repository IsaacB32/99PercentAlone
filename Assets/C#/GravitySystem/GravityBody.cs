using System;
using System.Linq;
using UnityEngine;

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
    
    [Header("Static Gravity Sources")]
    [Tooltip("GravitySources that are always calculated")]
    [SerializeField] protected GravitySource[] _staticGravitySources;
    
    //===== References =====
    public Rigidbody rb { get; private set; }
    public GravitySource[] Sources { get; private set; } = Array.Empty<GravitySource>();
    public float WeakestGravityStrength => _weakestGravityStrength;
    public float GravitySmoothingMax => _gravitySmoothingMax;
    
    protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        FindField();
    } 
 
    /// <summary>
    /// Check a unit sphere around the object to check if started inside a GravityField
    /// </summary>
    private void FindField()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 1, Layers.ToLayerMask(Layers.GravityField));
        foreach (Collider hit in hits)   
        {
            if (hit.isTrigger)
            {
                GravityField field = hit.GetComponent<GravityField>();
                field.ApplySourcesToBody(this);
            }
        }
    }

    protected void FixedUpdate()
    {
        ApplyGravity();
    }

    public void UpdateGravitySources(GravitySource[] sources)
    {
        if (Sources.Length != 0) Debug.LogWarning("GravitySources where changed unexpectedly");
        Sources = _staticGravitySources.Concat(sources).ToArray();
    }

    public void RemoveGravitySources()
    {
        Sources = Array.Empty<GravitySource>();
    }
    
    public virtual void ApplyGravity()
    {
        SpaceUtils.RelativeGravitySource closestSource = SpaceUtils.GetClosestSourceToObject(this);
        SpaceUtils.RotateObjectToSourceUp(this, closestSource);
    }
}
