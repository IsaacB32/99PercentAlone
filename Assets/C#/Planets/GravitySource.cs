using UnityEngine;

public abstract class GravitySource : MonoBehaviour
{
   public const float GRAVITATIONAL_CONST = 0.01f;
   
   //===== Settings =====

   [Header("Debug Visualize")]
   [SerializeField] private Transform _testObject;
   
   [Header("Basic Settings")]
   [SerializeField] protected float _size = 1f;
   [SerializeField] protected float _surfaceGravity = 1f;
   
   //===== Properties =====
   
   public virtual float Mass => _surfaceGravity * _size * _size / GRAVITATIONAL_CONST;
   
   //===== Calculations =====

   public abstract Vector3 VectorToCenter(Vector3 objectPosition);
   public abstract float DistanceToSurface(Vector3 objectPosition);
   
   //===== Visual =====

   protected abstract void ValidateSize();
   protected abstract void DrawIndicators(Transform testObject);
   
   //===== Private =====

   private void OnDrawGizmos()
   {
      Gizmos.color = Color.blue;
      DrawIndicators(_testObject);
   }

   private void OnValidate()
   {
      ValidateSize();
   }
}
