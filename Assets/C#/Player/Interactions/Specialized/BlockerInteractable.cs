using UnityEngine;

/// <summary>
/// An interactable that does nothing except block interactions
/// </summary>
public class BlockerInteractable : MonoBehaviour,
    IInteractable
{
    public void OnSelect()
    {
        //do nothing
    }
}
