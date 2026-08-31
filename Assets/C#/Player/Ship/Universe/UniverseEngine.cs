using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calculates the movement of the ship relative to all IUniverseBodies and moves them accordingly 
/// </summary>
public class UniverseEngine
{
    public static HashSet<IUniverseBody> AllBodies { get; private set; } = null;
    public static void AddUniverseBody(IUniverseBody body) { AllBodies.Add(body); }
    public static void RemoveUniverseBody(IUniverseBody body) { AllBodies.Remove(body); }
    public static void RemoveUniverseWhere(Predicate<IUniverseBody> match) { AllBodies.RemoveWhere(match); }
    
    public UniverseEngine(IUniverseBody[] initialBodies)
    {
        if (AllBodies != null) throw new Exception("There may only be one ShipMovementEngine");
        
        AllBodies = new HashSet<IUniverseBody>();
        foreach (IUniverseBody body in initialBodies) AllBodies.Add(body);
    }
    
    public void UpdateUniverse(Vector3 direction)
    {
        foreach (IUniverseBody universeBody in AllBodies)
        {
            universeBody.MoveBody(-direction);
        }
    }
}
