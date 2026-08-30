using UnityEngine;

/// <summary>
/// Process events when a GravityBody enters / exits
/// </summary>
public abstract class GravityEventTrigger : MonoBehaviour
{
    protected abstract GravityBodyType _bodyType { get; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out GravityBody body))
        {
            OnGravityBodyEnter(body);
            body.GravityType = _bodyType;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out GravityBody body))
        {
            OnGravityBodyExit(body);
            body.GravityType = GravityBodyType.Reassign;
        }
    }

    /// <summary>
    /// GravityBody enters the trigger
    /// </summary>
    public abstract void OnGravityBodyEnter(GravityBody body);

    /// <summary>
    /// GravityBody exits the trigger
    /// </summary>
    protected abstract void OnGravityBodyExit(GravityBody body);

    protected virtual void OnValidate()
    {
        gameObject.layer = Layers.GravityTrigger;
    }
}

