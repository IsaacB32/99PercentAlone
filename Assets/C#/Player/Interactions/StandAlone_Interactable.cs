using UnityEngine;

/// <summary>
/// Interactable designed to only be used for triggering its callback event, used for simple selections or debugging 
/// </summary>
public class StandAlone_Interactable : Interactable
{
    protected override void OnSelect()
    {
        //do nothing 
    }
}
