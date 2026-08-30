using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Catches input from the PlayerInput component and distributes it to player scripts 
/// </summary>
public class PlayerInputController : InputController
{
    public enum MovementType
    {
        Gravity,
        Weightless,
    }

    [Space]
    [SerializeField] private MovementType _enteringState;
    private MovementType _currentMovementType;
    public MovementType CurrentMovementType
    {
        get => _currentMovementType;
        set
        {
            if (_currentMovementType == value) return;

            OnSwitchInputState?.Invoke(value);
            _currentMovementType = value;
        }
    }
    
    [field: Header("References")]
    [field: SerializeField] public Transform CameraOriginReference { get; private set; }
    [field: SerializeField] public PlayerMovement Input_PlayerMovement { get; private set; }
    [field: SerializeField] public PlayerCamera Input_PlayerCamera { get; private set; } 
    
    public event Action<MovementType> OnSwitchInputState;
    
    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundDistance;

    [Header("Camera Settings")]
    [SerializeField] private bool _ignoreInputWhenCursorUnlocked = true;
    [SerializeField] private bool _invertMouse = false;
    [SerializeField] private float _mouseSensitivityX = 0.8f;
    [SerializeField] private float _mouseSensitivityY = 0.8f;
    [Range(0f, 1f)] [SerializeField] private float _aimSensitivityMultiplier = 0.5f;
    
    //===== Input Properties =====
    public Vector3 MovementInput { get; private set; }
    public Vector3 MouseDelta { get; private set; }
    
    //===== Callbacks =====
    public event Action<bool> OnJump;
    
    //===== Toggles =====
    public bool IsGrounded { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsAiming { get; private set; }

    //===== Pressed this Frame =====
    private int _interactPressedTime;
    public bool InteractPressedThisFrame => _interactPressedTime == Time.frameCount;
    
    //===== State Machine =====
    
    public override void OnEnter(InputMapType oldType)
    {
        Input_PlayerMovement.SetEnablePhysics(true);
        IsUpdateLocked = false;
    }

    public override void OnExit(InputMapType newType)
    {
        IsUpdateLocked = true;
        ResetValues();
        Input_PlayerMovement.SetEnablePhysics(false);
    }

    //===== Input Action Callbacks =====
    
    public void Movement(InputAction.CallbackContext context)
    {
        if (InputEngine.InputLock) return;
        
        IsMoving = true;
        if (context.canceled) IsMoving = false;

        Vector2 movement = context.ReadValue<Vector2>();
        MovementInput = new Vector3(movement.x, 0f, movement.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (InputEngine.InputLock) return;
        
        if (context.performed)
        {
            OnJump?.Invoke(true);
            IsGrounded = false;
        }

        if (context.canceled)
        {
            OnJump?.Invoke(false);
        }
    }

    public void Running(InputAction.CallbackContext context)
    {
        if (InputEngine.InputLock) return;
        
        if (context.performed) IsRunning = true;
        else if (context.canceled) IsRunning = false;
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (InputEngine.InputLock) return;
        
        if (context.performed) _interactPressedTime = Time.frameCount;
    }

    public void Aim(InputAction.CallbackContext context)
    {
        if (InputEngine.InputLock) return;
        
        if (context.performed) IsAiming = true;
        else if (context.canceled) IsAiming = false;
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (InputEngine.InputLock) return;
        
        // Skip when the cursor is unlocked so menus don't yank the camera around
        if (_ignoreInputWhenCursorUnlocked && Cursor.lockState != CursorLockMode.Locked)
        {
            MouseDelta = Vector2.zero;
            return;
        }

        Vector2 mouse = context.ReadValue<Vector2>();
        float mouseX = mouse.x * _mouseSensitivityX;
        float mouseY = mouse.y * _mouseSensitivityY;

        if (IsAiming)
        {
            mouseX *= _aimSensitivityMultiplier;
            mouseY *= _aimSensitivityMultiplier;
        }

        if (_invertMouse) mouseY = -mouseY;

        Vector2 activeMouse = new Vector2(mouseX, mouseY);
        MouseDelta = activeMouse;
    }
    
    //===== External Callers =====
    //methods that are called by other scripts to modify input/position

    /// <summary>
    /// Move the player position to a point
    /// </summary>
    /// <param name="point">point to move to</param>
    /// <param name="cameraMove">should the camera move with the player</param>
    public void SnapPlayerPosition(Vector3 point, bool cameraMove = true)
    {
        Vector3 dis = Input_PlayerMovement.RB.position - point;
        Input_PlayerMovement.RB.position = transform.position = point;
        if (!cameraMove)
        {
            Input_PlayerCamera.transform.position += dis;
        }
    }
    
    /// <summary>
    /// Move the player to a point and rotate the camera to look in a direction
    /// </summary>
    /// <param name="point">point to move to</param>
    /// <param name="cameraRotation">rotation of camera after snap</param>
    /// <param name="cameraMove">should the camera move with the player</param>
    public void SnapPlayerPosition(Vector3 point, Vector3 cameraRotation, bool cameraMove = true)
    {
        //todo: if needed
    }
    
    //===== Other =====

    private void ResetValues()
    {
        IsMoving = false;
        IsRunning = false;
        IsAiming = false;
        MovementInput = Vector3.zero;
        MouseDelta = Vector3.zero;
        
        Input_PlayerMovement.ZeroOutMovementVector();
        Input_PlayerCamera.RecenterView();
    }
    
    private void FixedUpdate()
    {
        IsGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, InputEngine.GroundLayer);
    }

    //===== DEBUGGING =====

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && _groundCheck != null)
        {
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(_groundCheck.position, _groundDistance);
        }
    }
}