using UnityEngine;

public class UniverseBodyTransform : UniverseBody
{
    public override void MoveBody(Vector3 position)
    {
        transform.position = position;
    }
}
