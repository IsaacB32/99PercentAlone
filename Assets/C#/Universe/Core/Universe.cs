using System;
using System.Collections.Generic;
using Isaac.Extensions;
using UnityEngine;

/// <summary>
/// The Universe controller: is the core of the Universe system
/// </summary>
[DefaultExecutionOrder(-1000)]
public class Universe : MonoBehaviour
{
    //=!= Lots of stuff in the code requires the origin be zero, DO NOT CHANGE =!=
    public static readonly Vector3 WorldOrigin = Vector3.zero;
    
    [SerializeField] private Transform _playerEngine;
    
    [Space]
    [SerializeField, EditorOnly] private UniverseCoordinates _universeOrigin = UniverseCoordinates.zero;
    [Tooltip("Every _universeRadius units is how much Universe Coordinates")]
    [SerializeField, EditorOnly] private int _coordMagnitude;
    [SerializeField, EditorOnly] private float _universeRadius = 10f;

    //===== Bodies =====
    public HashSet<UniverseBody> UniverseBodies { get; private set; } = null;
    public void AddUniverseBody(UniverseBody body) { UniverseBodies.Add(body); }
    public void RemoveUniverseBody(UniverseBody body) { UniverseBodies.Remove(body); }
    public void RemoveUniverseWhere(Predicate<UniverseBody> match) { UniverseBodies.RemoveWhere(match); }

    //===== Properties =====
    
    public float UniverseRadius => _universeRadius; 
    public UniverseCoordinates CurrentUniverseCenter { get; private set; }
    public int UniverseGridSize => (int)_universeRadius / _coordMagnitude;
    
    //===== Callbacks =====

    public event Action OnUniverseUpdated;
    
    //=!= Expensive =!=
    /// <summary>
    /// Scan through the entire scene and find all IUniverseBodies
    /// </summary>
    public static UniverseBody[] SweepForUniverseBodies => FindObjectsByType<UniverseBody>();

    private void Awake()
    {
        UniverseBodies = new HashSet<UniverseBody>();
        foreach (UniverseBody body in SweepForUniverseBodies) { UniverseBodies.Add(body); }

        CurrentUniverseCenter = _universeOrigin;
        UpdateUniverse();
    }

    private void LateUpdate()
    {
        Vector3 position = _playerEngine.position;
        if (position.sqrMagnitude > _universeRadius.sqr())
        {
            UniverseCoordinates offset = new UniverseCoordinates(position.normalized * (UniverseGridSize / 2f));
            CurrentUniverseCenter += offset;

            Vector3 worldOffset = (Vector3)offset * UniverseGridSize;
            Vector3 newShipPos = position - worldOffset;
            
            UpdateUniverse();
            _playerEngine.position = newShipPos;
            Physics.SyncTransforms();
        }
    }
    
    public void UpdateUniverse()
    {
        foreach (UniverseBody universeBody in UniverseBodies)
        {
            universeBody.MoveBody(UniverseToWorld(universeBody.Coords));
        }
        OnUniverseUpdated?.Invoke();
    }
    
    //===== Coord Conversions =====
    
    public UniverseCoordinates WorldToUniverse(Vector3 objectPos)
    {
        UniverseCoordinates local = new UniverseCoordinates(objectPos / UniverseGridSize);
        local += CurrentUniverseCenter;
        return local;
    }
    
    public Vector3 UniverseToWorld(UniverseCoordinates universeCoords)
    {
        UniverseCoordinates localCoords = universeCoords - CurrentUniverseCenter;
        return (Vector3)(localCoords * UniverseGridSize);
    }

    //===== Inspector =====
    
    [ContextMenu("Add Grid")]
    private void AddGrid()
    {
        Modifier.AddModifier<Universe, UniverseDrawer>(this);
    }
    
    [ContextMenu("Add Loader")]
    private void AddLoader()
    {
        Modifier.AddModifier<Universe, UniverseLoader>(this);
    }
}
