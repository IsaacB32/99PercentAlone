using NaughtyAttributes;
using UnityEngine;

//=!= IDEA ONLY =!=
// deemed not useful

/// <summary>
/// Abstract class for assigning Modifiers which will extend the functionality of existing classes without overriding them
/// </summary>
public abstract class Modifier<T> : MonoBehaviour where T : MonoBehaviour
{
    [field: Required("Target is required")]
    [field: SerializeField] public T Target { get; private set; }

    protected void Reset()
    {
        //force assign Target if null
        if (Target == null) Target = GetComponent<T>();
    }

    public abstract void Apply();
}
