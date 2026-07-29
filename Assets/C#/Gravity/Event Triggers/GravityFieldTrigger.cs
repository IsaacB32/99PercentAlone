using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Updates the GravityBody sources when triggered
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class GravityFieldTrigger : GravityEventTrigger
{
    [SerializeField] private float _radius = 20f;
    protected override GravityBodyType _bodyType => GravityBodyType.Source;
    
    private GravitySource[] _sources;

    private void Awake()
    {
        _sources = GetComponentsInChildren<GravitySource>();
    }

    public override void OnGravityBodyEnter(GravityBody body)
    {
        body.UpdateGravitySources(_sources); 
    }

    protected override void OnGravityBodyExit(GravityBody body)
    {
        body.RemoveGravitySources();
    }

    /// <summary>
    /// Update the _radius to the farthest GravitySource
    /// </summary>
    [Button("Update Radius")]
    private void FindFarthestSource()
    {
        float largestDistance = 0f;
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (GravitySource source in _sources)
        {
            if (!source) continue;
            float distance = Vector3.Distance(transform.position, source.transform.position + (source.transform.position.normalized * source.Size));
            if (distance > largestDistance) largestDistance = distance;
        }

        _radius = largestDistance;
        OnValidate();
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        
        SphereCollider collider = GetComponent<SphereCollider>();
        collider.radius = _radius;
        collider.isTrigger = true;
    }
}

