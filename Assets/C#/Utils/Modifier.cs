using System;
using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Abstract class for assigning Modifiers which will extend the functionality of existing classes without overriding them
/// </summary>
public abstract class Modifier<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField, ReadOnly, Required("Not Valid, call Reset or add again")] protected T _target;
    
    public virtual void Initialize(T taget)
    {
        _target = taget;
    }

    protected virtual void Awake()
    {
        _target = GetComponent<T>();
        if (_target == null) throw new Exception("Modifier left without target reference!");
    }

    private void Reset()
    {
        _target = GetComponent<T>();
    }
}

public static class Modifier
{
    public static void AddModifier<TTarget, TComponent>(TTarget targetObject) 
        where TTarget : MonoBehaviour
        where TComponent : Modifier<TTarget> 
    {
        if (!targetObject.TryGetComponent(out TComponent component))
        {
            component = targetObject.gameObject.AddComponent<TComponent>();
        }
        component.Initialize(targetObject);
    }
}
