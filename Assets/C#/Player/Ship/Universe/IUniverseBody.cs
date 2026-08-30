using UnityEngine;

/// <summary>
/// Interface to allow objects to move when the ship moves 
/// </summary>
public interface IUniverseBody
{
    /// <summary>
    /// How the UniverseBody moves along a distance
    /// </summary>
    public void MoveBody(Vector3 movement);
}
