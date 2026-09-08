using UnityEngine;

/// <summary>
/// ScriptableObject container for all the Universe objects  
/// </summary>
public class UniverseStorage : ScriptableObject
{
    [SerializeField] private UniverseLoader.UniverseObject[] _allObjects;
    public UniverseLoader.UniverseObject[] AllObjects => _allObjects;
}
