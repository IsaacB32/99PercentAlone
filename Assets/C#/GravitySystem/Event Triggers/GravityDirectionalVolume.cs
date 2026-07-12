using UnityEngine;

/// <summary>
/// Directional gravity in a volume
/// </summary>
public class GravityDirectionalVolume : GravityDirectionalTrigger
{
    protected override void ConfirmSize(BoxCollider gravityEffector)
    {
        gravityEffector.size = new Vector3(1f, _gravityEffectorThickness, 1f);
    }
}
