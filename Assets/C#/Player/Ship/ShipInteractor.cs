using UnityEngine;

public class ShipInteractor : Interactable
{
    [SerializeField] private ShipController _controller;
    
    public override void OnSelect()
    {
        _controller.ControlShip();
    }

    protected override void OnHoverEnter()
    {
        
    }

    protected override void OnHoverExit()
    {
        
    }
}
