using System;
using System.Linq;
using CustomAttributes;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// A collection of GravitySources that will set as the objects gravity objects
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class GravityField : MonoBehaviour
{
    [SerializeField] private float _radius = 20f;
    [SerializeField] private GravitySource[] _sources;
    
    public void ApplySourcesToBody(GravityBody body) { body.UpdateGravitySources(_sources); }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out GravityBody body))
        {
            body.UpdateGravitySources(_sources);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out GravityBody body))
        {
            body.RemoveGravitySources();
        }
    }

    /// <summary>
    /// Update the _radius to the farthest GravitySource
    /// </summary>
    [FunctionButton("Update Radius")]
    private void FindFarthestSource()
    {
        float largestDistance = 0f;
        foreach (GravitySource source in _sources)
        {
            if (!source) continue;
            float distance = Vector3.Distance(transform.position, source.transform.position + (source.transform.position.normalized * source.Size));
            if (distance > largestDistance) largestDistance = distance;
        }

        _radius = largestDistance;
        OnValidate();
    }

    /// <summary>
    /// Assign all children GravitySource to _sources 
    /// </summary>
    [FunctionButton("Assign Children")]
    private void AssignChildrenToSource()
    {
        GravitySource[] childrenSources = transform.GetComponentsInChildren<GravitySource>();
        _sources = _sources.Concat(childrenSources).ToArray();
        FindFarthestSource();
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    private void OnValidate()
    {
        SphereCollider collider = GetComponent<SphereCollider>();
        collider.radius = _radius;
        collider.isTrigger = true;
    }
}

