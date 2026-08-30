using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calculates the movement of the ship relative to all IUniverseBodies and moves them accordingly 
/// </summary>
public class ShipMovementEngine
{
    public static HashSet<IUniverseBody> AllBodies { get; private set; } = null;
    public static void AddUniverseBody(IUniverseBody body) { AllBodies.Add(body); }
    public static void RemoveUniverseBody(IUniverseBody body) { AllBodies.Remove(body); }
    public static void RemoveUniverseWhere(Predicate<IUniverseBody> match) { AllBodies.RemoveWhere(match); }

    private MovementParameters _movementParameters;
    public struct MovementParameters
    {
        public float movementSpeed;
    }
    
    public ShipMovementEngine(MovementParameters parameters, IUniverseBody[] initialBodies)
    {
        if (AllBodies != null) throw new Exception("There may only be one ShipMovementEngine");
        
        AllBodies = new HashSet<IUniverseBody>();
        _movementParameters = parameters;
        
        foreach (IUniverseBody body in initialBodies) AllBodies.Add(body);
    }
    
    public void UpdateUniverse(Vector3 movement, Transform relativeBody)
    {
        foreach (IUniverseBody body in AllBodies)
        {
            Vector3 movementVector = movement * _movementParameters.movementSpeed;
            Vector3 relativeVector = relativeBody.TransformPoint(movementVector);
            body.UniverseHandle.position -= relativeVector * Time.fixedDeltaTime;
        }
    }
}
