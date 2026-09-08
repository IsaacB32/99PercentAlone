using System;
using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Allow objects to move when the universe is updated
/// </summary>
public abstract class UniverseBody : MonoBehaviour
{
    /// <summary>
    /// Coords explaining the location of the Body in the Universe
    /// </summary>
    [field: SerializeField] public UniverseCoordinates Coords { get; protected set; }
    
    /// <summary>
    /// How the UniverseBody moves to a new position
    /// </summary>
    public abstract void MoveBody(Vector3 position);
    
    //===== Setup =====

    private void Reset()
    {
        UpdateUniverseCoords();
    }

    public void Initialize(UniverseCoordinates coords)
    {
        Coords = coords;
        UpdateWorldCoords();
    }

    //===== Inspector =====
    
    /// <summary>
    /// Universe Coords are updated based on World Position 
    /// </summary>
    [Button]
    private void UpdateUniverseCoords()
    {
        Universe universe = FindAnyObjectByType<Universe>();
        if (universe != null)
        {
            Coords = universe.WorldToUniverse(transform.position);
        }
    }

    /// <summary>
    /// World Position is updated based on Universe Coords
    /// </summary>
    [Button]
    private void UpdateWorldCoords()
    {
        Universe universe = FindAnyObjectByType<Universe>();
        if (universe != null)
        {
            MoveBody(universe.UniverseToWorld(Coords));
        }
    }
}
