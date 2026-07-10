using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputCatcher : MonoBehaviour
{
    public static InputCatcher Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Debug.LogWarning($"two copies of {this} present : {gameObject.name}");
            Destroy(gameObject);
        }

        CurrentInputState = _enteringState;
    }

    [SerializeField] private PlayerInputState _enteringState;
    private PlayerInputState _currentPlayerInputState;
    public static PlayerInputState CurrentInputState
    {
        get => Instance._currentPlayerInputState;
        set
        {
            if (Instance._currentPlayerInputState == value) return;
            
            // Debug.Log($"Entering Input State - {value}");
            OnSwitchInputState?.Invoke(value);
            Instance._currentPlayerInputState = value;
        }
    }
    public static event Action<PlayerInputState> OnSwitchInputState;
    
    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundDistance;
    [SerializeField] private LayerMask _groundMask;

    [Header("Camera Settings")]
    [SerializeField] private bool _ignoreInputWhenCursorUnlocked = true;
    [SerializeField] private bool _invertMouse = false;
    [SerializeField] private float _mouseSensitivityX = 0.8f;
    [SerializeField] private float _mouseSensitivityY = 0.8f;
    [Range(0f, 1f)] [SerializeField] private float _aimSensitivityMultiplier = 0.5f;
    // [Range(0f, 1f)] [SerializeField] private float _aimShakeMultiplier = 0.3f;
    
    //===== Input Properties =====

    private Vector3 _movementVector;
    public static Vector3 MovementVector => Instance._movementVector;
    
    public static event Action<Vector3> OnMove;
    public static event Action<Vector2> OnLook;
    public static event Action OnJumpPressed;
    public static event Action OnJumpReleased;
    
    private bool _isGrounded;
    public static bool IsGrounded => Instance._isGrounded; 

    private bool _isMoving;
    public static bool IsMoving => Instance._isMoving;
    
    private bool _isRunning;
    public static bool IsRunning => Instance._isRunning;
    
    private bool _isAiming;
    public static bool IsAiming => Instance._isAiming;
    

    public void Movement(InputAction.CallbackContext context)
    {
        _isMoving = true;
        if (context.canceled) _isMoving = false;

        Vector2 movement = context.ReadValue<Vector2>();
        _movementVector = new Vector3(movement.x, 0f, movement.y);
        OnMove?.Invoke(_movementVector);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnJumpPressed?.Invoke();
            _isGrounded = false;
        }

        if (context.canceled)
        {
            OnJumpReleased?.Invoke();
        }
    }

    public void Running(InputAction.CallbackContext context)
    {
        if (context.performed) _isRunning = true;
        else if (context.canceled) _isRunning = false;
    }

    public void Interact(InputAction.CallbackContext context)
    {
        
    }

    public void Aim(InputAction.CallbackContext context)
    {
        if (context.performed) _isAiming = true;
        else if (context.canceled) _isAiming = false;
    }

    public void Look(InputAction.CallbackContext context)
    {
        // Skip when the cursor is unlocked so menus don't yank the camera around
        if (_ignoreInputWhenCursorUnlocked && Cursor.lockState != CursorLockMode.Locked)
        {
            // _lookVector = Vector2.zero;
            OnLook?.Invoke(Vector2.zero);
            return;
        }

        Vector2 mouse = context.ReadValue<Vector2>();
        float mouseX = mouse.x * _mouseSensitivityX;
        float mouseY = mouse.y * _mouseSensitivityY;

        if (_isAiming)
        {
            mouseX *= _aimSensitivityMultiplier;
            mouseY *= _aimSensitivityMultiplier;
        }

        if (_invertMouse) mouseY = -mouseY;

        // _lookVector = new Vector2(mouseX, mouseY);
        OnLook?.Invoke(new Vector2(mouseX, mouseY));
    }
    
    //===== Other =====
    
    private void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
        
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
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