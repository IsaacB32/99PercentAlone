using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipInputController : InputController
{
   [field: Header("References")]
   [field: SerializeField] public ShipMovement Input_ShipMovement { get; private set; }
   
   [Space]
   [SerializeField] private float _maxSpeed = 10f;
   [SerializeField] private float _movementSpeed = 2f;

   private Vector3 _movementVector;
   
   //===== Callbacks =====
   public event Action OnEndControlShip;
   
   //===== State Machine =====
   
   public override void OnEnter(InputMapType oldType)
   {
      IsUpdateLocked = false;
   }

   public override void OnExit(InputMapType newType)
   {
      IsUpdateLocked = true;
   }

   //===== Input Action Callbacks =====
   
   //TODO: move to the ShipMovementEngine to calculate how to move the world instead of the player 
   public void Movement(InputAction.CallbackContext context)
   {
      if (InputEngine.HasInputLock) return;
   }

   public void Look(InputAction.CallbackContext context)
   {
      if (InputEngine.HasInputLock) return;
   }

   public void Boost(InputAction.CallbackContext context)
   {
      if (InputEngine.HasInputLock) return;

      if (context.performed) OnEndControlShip?.Invoke();
   }
   
   //===== Force Apply =====

   private void FixedUpdate()
   {
      transform.position += _movementVector * Time.fixedDeltaTime;
   }
}
