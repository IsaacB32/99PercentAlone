using System;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [SerializeField] private ShipInputController _shipInputController;
    [SerializeField] private Interactable _shipInteractable;
    [SerializeField] private ShipCameraAnimator _shipCameraAnimator;
    

    #region Subscribe

    private void OnEnable()
    {
        _shipInteractable.OnSelected += OnStartControlShip;
        _shipInputController.OnEndControlShip += OnEndControlShip;
    }

    private void OnDisable()
    {
        _shipInteractable.OnSelected -= OnStartControlShip;
        _shipInputController.OnEndControlShip -= OnEndControlShip;
    }

    #endregion
    
    private void OnStartControlShip()
    {
        _shipCameraAnimator.AnimateToTarget();
    }

    private void OnEndControlShip()
    {
        _shipCameraAnimator.AnimateToOrigin(() =>
        {
            InputEngine.SwitchActionMap(InputMapType.Player);
        });
    }

}
