using System;
using System.Collections.Generic;
using Isaac.Extensions;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Modifier for Universe: loads and unloads objects depending on distance from them 
/// </summary>
public class UniverseLoader : Modifier<Universe>
{
    [Space]
    [SerializeField] private float _loadRange = 100f;
    [SerializeField] private float _unloadBuffer = 10f;
    
    [Space]
    [SerializeField, Expandable] private UniverseStorage _universeObjects;

    private Dictionary<string, UniverseBody> _activeObjects = new Dictionary<string, UniverseBody>();
    
    #region Subscribe

    private void OnEnable()
    {
        _target.OnUniverseUpdated += OnUniverseUpdated;
    }

    private void OnDisable()
    {
        _target.OnUniverseUpdated -= OnUniverseUpdated;
    }

    #endregion

    private void OnUniverseUpdated()
    {
        LoadObjects();
        UnloadObjects();
    }

    private void LoadObjects()
    {
        List<UniverseObject> toLoadList = new List<UniverseObject>();
        
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (UniverseObject universeObject in _universeObjects.AllObjects)
        {
            float distance = UniverseCoordinates.SqrDistance(universeObject.location, _target.CurrentUniverseCenter);
            if (distance <= _loadRange.sqr()) toLoadList.Add(universeObject);
        }

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (UniverseObject universeObject in toLoadList)
        {
            if (!_activeObjects.ContainsKey(universeObject.key)) loadObject(universeObject);
        }
        return;
        
        void loadObject(UniverseObject toLoad)
        {
            AsyncOperationHandle loadHandle = Addressables.InstantiateAsync(toLoad.body);
            loadHandle.Completed += handle =>
            {
                if (handle.Status != AsyncOperationStatus.Succeeded) return;
                
                GameObject result = (GameObject)handle.Result;
                if (!result.TryGetComponent(out UniverseBody universeBody))
                {
                    universeBody = result.AddComponent<UniverseBodyTransform>();
                }
                universeBody.Initialize(toLoad.location);

                _activeObjects.Add(toLoad.key, universeBody);
                _target.AddUniverseBody(universeBody);
            };
        }
    }

    private void UnloadObjects()
    {
        List<string> toUnloadKeys = new List<string>();
        
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (KeyValuePair<string, UniverseBody> active in _activeObjects)
        {
            float distance = UniverseCoordinates.SqrDistance(active.Value.Coords, _target.CurrentUniverseCenter);
            if (distance > (_loadRange + _unloadBuffer).sqr()) toUnloadKeys.Add(active.Key);
        }
        
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (string unloadKey in toUnloadKeys)
        {
            unloadObject(unloadKey);
        }
        return;

        void unloadObject(string toUnloadKey)
        {
            if (Addressables.ReleaseInstance(_activeObjects[toUnloadKey].gameObject))
            {
                _target.RemoveUniverseBody(_activeObjects[toUnloadKey]);
                _activeObjects.Remove(toUnloadKey);
            }
            else Debug.LogWarning($"UniverseLoader failed to clean up object {toUnloadKey}");
        }
    }
    
    [Serializable]
    public struct UniverseObject
    {
        public string name;
        public UniverseCoordinates location; 
        public AssetReferenceGameObject body;
        public string key => $"{name}_{body}";
    }
}
