using UnityEngine;

public interface IModifier<T>
{
    
    public void Initialize(T target);
}
