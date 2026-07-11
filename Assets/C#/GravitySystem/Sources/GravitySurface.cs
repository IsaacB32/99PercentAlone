using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GravitySurface : GravitySource
{
    [Header("Surface Settings")]
    [SerializeField] private float _width = 1f;
    [SerializeField] private float _gravityEffectorThickness = 1f;
    
    public override Vector3 VectorToCenter(Vector3 objectPosition)
    {
        throw new System.NotImplementedException();
    }

    public override float DistanceToSurface(Vector3 objectPosition)
    {
        throw new System.NotImplementedException();
    }

    protected override void ValidateSize()
    {
        transform.localScale = new Vector3(_size, 1f, _width);

        BoxCollider gravityEffector = GetComponent<BoxCollider>();
        gravityEffector.isTrigger = true;
        
        gravityEffector.size = new Vector3(gravityEffector.size.x, _gravityEffectorThickness, gravityEffector.size.z);
        gravityEffector.center = Vector3.up * (_gravityEffectorThickness / 2f);
    }
}
