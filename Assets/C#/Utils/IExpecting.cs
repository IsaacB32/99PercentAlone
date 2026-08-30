using UnityEngine;

//=!= IDEA ONLY =!=
// deemed not useful

/// <summary>
/// Interface for forcing a class to expect a value so it can be uses as a Delegate
/// </summary>
public interface IExpecting<T> where T : class
{
    public T Delegate { get; set; }
}
