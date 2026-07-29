using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Directional gravity on a surface, usually with ground collision attached
/// </summary>
public class GravityDirectionalSurface : GravityDirectionalTrigger
{
    protected override void ConfirmSize(BoxCollider gravityEffector)
    {
        gravityEffector.size = new Vector3(10f, _gravityEffectorThickness, 10f);
        gravityEffector.center = Vector3.up * (_gravityEffectorThickness / 2f);
    }
    
    //===== Inspector Util =====

    /// <summary>
    /// Move this script and BoxCollider to a new child object if ground collisions are required on the parent
    /// </summary>
    [Button]
    private void MigrateToChild()
    {
        GameObject child = new GameObject("Directional Trigger")
        {
            transform =
            {
                parent = transform,
                localPosition = Vector3.zero,
                localRotation = Quaternion.identity,
                localScale = Vector3.one
            }
        };

        gameObject.layer = Layers.Ground;
        child.AddComponent<GravityDirectionalSurface>().CopyParametersFrom(this);
    }
}
