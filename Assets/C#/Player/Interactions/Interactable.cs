using System;
using UnityEngine;

/// <summary>
/// Class to inherit from, allows interactions with Interactor
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    public event Action OnSelected;
    
   //===== Current Hovering =====

    /// <summary>
    /// Currently hovering object in the game
    /// </summary>
    public static Interactable CurrentHovering { get; private set; }
    
    /// <summary>
    /// Updates the CurrentHovering and calls OnHover actions
    /// </summary>
    public static void RefreshHovering(Interactable interactable)
    {
        if (!Equals(CurrentHovering, interactable))
        {
            CurrentHovering?.OnHoverExit();
            interactable?.OnHoverEnter();
        }
        else CurrentHovering?.OnHoverStay();
        
        CurrentHovering = interactable;
    }

    /// <summary>
    /// Resets the CurrentHovering with no callbacks for OnHover actions
    /// </summary>
    public static void ClearCurrentHovering()
    {
        CurrentHovering = null;
    }
    
    //===== Interactions =====

    public void Select()
    {
        OnSelect();
        OnSelected?.Invoke();
    }

    protected abstract void OnSelect();
    
    protected virtual void OnHoverEnter() {}
    protected virtual void OnHoverStay() {}
    protected virtual void OnHoverExit() {}

    
    private void OnValidate()
    {
        gameObject.layer = Layers.Interaction;
    }
}
