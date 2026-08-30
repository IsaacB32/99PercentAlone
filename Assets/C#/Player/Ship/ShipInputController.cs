using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipInputController : InputController
{
   [field: Header("References")]
   [field: SerializeField] public ShipMovement Input_ShipMovement { get; private set; }
   
   [Header("Camera Settings")]
   [SerializeField] private bool _invertMouse = false;
   [SerializeField] private float _mouseSensitivityX = 0.8f;
   [SerializeField] private float _mouseSensitivityY = 0.8f;
   
   //===== Input Properties =====
   public Vector3 MovementInput { get; private set; }
   public Vector3 MouseDelta { get; private set; }
   
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
   
   public void Movement(InputAction.CallbackContext context)
   {
      if (InputEngine.InputLock) return;

      Vector2 movement = context.ReadValue<Vector2>();
      MovementInput = new Vector3(movement.x, 0f, movement.y);
   }

   public void Look(InputAction.CallbackContext context)
   {
      if (InputEngine.InputLock) return;
      
      // Skip when the cursor is unlocked so menus don't yank the camera around
      if (Cursor.lockState != CursorLockMode.Locked)
      {
         MouseDelta = Vector2.zero;
         return;
      }
      
      Vector2 mouse = context.ReadValue<Vector2>();
      float mouseX = mouse.x * _mouseSensitivityX;
      float mouseY = mouse.y * _mouseSensitivityY;

      if (_invertMouse) mouseY = -mouseY;

      Vector2 activeMouse = new Vector2(mouseX, mouseY);
      MouseDelta = activeMouse;
   }

   public void Boost(InputAction.CallbackContext context)
   {
      if (InputEngine.InputLock) return;

      if (context.performed)
      {
         MouseDelta = Vector3.zero;
         OnEndControlShip?.Invoke();
      }
   }
}
