using System;
using UnityEngine;

/// <summary>
/// Interactable designed to only be used for triggering its callback event, used for simple selections or debugging 
/// </summary>
public class StandAloneInteractable : MonoBehaviour, IInteractable
{
    public event Action Select;
    
    public void OnSelect()
    {
        Select?.Invoke(); 
    }
}
