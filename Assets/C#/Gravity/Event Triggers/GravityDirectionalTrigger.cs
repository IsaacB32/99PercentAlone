using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Switches the gravity mode to Directional when triggered 
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public abstract class GravityDirectionalTrigger : GravityEventTrigger
{
    [SerializeField] private Vector3 _gravityDirection;
    [SerializeField] protected float _gravityEffectorThickness = 1f;

    protected override GravityBodyType _bodyType => GravityBodyType.Directional;

    //===== Collision =====

    public override void OnGravityBodyEnter(GravityBody body)
    {
        body.DirectionalGravity = _gravityDirection.normalized;
    }

    protected override void OnGravityBodyExit(GravityBody body)
    {
        body.DirectionalGravity = Vector3.zero;
    }
    
    //===== Update Direction =====

    public void SetNewGravityDirection(Vector3 direction)
    {
        _gravityDirection = direction;
    }

    public void SetGravityToUp()
    {
        _gravityDirection = transform.up;
    }

    //===== Visual =====
    
    [Button("Reset Gravity Direction")]
    protected void Reset()
    {
        SetGravityToUp();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 endPoint = (transform.position + _gravityDirection.normalized * 5f);
        Gizmos.DrawLine(transform.position, endPoint);
        Gizmos.DrawSphere(endPoint, 0.15f);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        
        BoxCollider gravityEffector = GetComponent<BoxCollider>();
        gravityEffector.isTrigger = true;
        ConfirmSize(gravityEffector);
    }

    protected abstract void ConfirmSize(BoxCollider gravityEffector);
    
#if UNITY_EDITOR
    
    //===== Inspector Util =====
    
    /// <summary>
    /// Copies the parameters and then deletes the original
    /// </summary>
    protected void CopyParametersFrom(GravityDirectionalTrigger original)
    {
        _gravityDirection = original._gravityDirection;
        _gravityEffectorThickness = original._gravityEffectorThickness;
        
        OnValidate();
        BoxCollider reference = original.GetComponent<BoxCollider>();
        DestroyImmediate(original);
        DestroyImmediate(reference);
    }
    
#endif
    
}