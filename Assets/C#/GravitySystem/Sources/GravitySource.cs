using UnityEngine;
using CustomAttributes;

/// <summary>
/// Source of gravity
/// </summary>
public abstract class GravitySource : MonoBehaviour
{
    //===== Settings =====

    [Header("Debug Visualize")]
    [SerializeField] protected GravityBody _testObject;
   
    [Header("Settings")]
    [SerializeField] protected float _surfaceGravity = 1f;
    
    [Space]
    [SerializeField] protected float _size = 1f;
    
    //===== Properties =====

    public virtual float Size => _size;
    public virtual float Mass => _surfaceGravity * _size * _size / SpaceUtils.GRAVITATIONAL_CONST;
    
    //===== Calculations =====

    /// <summary>
    /// Vector from the objectPosition to the center of the source
    /// </summary>
    public abstract Vector3 VectorToCenter(Vector3 objectPosition);
   
    /// <summary>
    /// Distance from the objectPosition to the surface of the source
    /// </summary>
    public abstract float DistanceToSurface(Vector3 objectPosition);
    
    //===== Visual =====
    
    /// <summary>
    /// Ensure the size of the transform matches the size of the variables 
    /// </summary>
    [FunctionButton("Set Size")]
    protected abstract void ValidateSize();
    
    /// <summary>
    /// Draw the gravity influence from the testObject to the source
    /// </summary>
    protected virtual void DrawIndicators(Transform testObject) { Debug.LogWarning("Not implemented");}
    
    //===== Private =====

    private void OnDrawGizmos()
    {
        if (!_testObject) return;
      
        Gizmos.color = Color.blue;
        DrawIndicators(_testObject.transform);
    }
}