using System;
using UnityEngine;

/// <summary>
/// Interface to mark as interactable allows interactions with Interactor, requires Interaction Layer to work
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Updates the CurrentHovering and calls OnHover actions
    /// </summary>
    public static void RefreshHovering(IInteractable iInteractable)
    {
        if (!Equals(Interactor.CurrentHovering, iInteractable))
        {
            Interactor.CurrentHovering?.OnHoverExit();
            iInteractable?.OnHoverEnter();
        }
        else Interactor.CurrentHovering?.OnHoverStay();
    }
    
    //===== Interactions =====
    
    public void OnSelect();
    
    protected void OnHoverEnter() {}
    protected void OnHoverStay() {}
    protected void OnHoverExit() {}
}

//=!= If issues check layer: Requires 'Interaction' =!=
