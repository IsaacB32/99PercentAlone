using System;
using UnityEngine;

public class PlayerMovement : GravityBody
{
    [Header("Controller")]
    [SerializeField] private PlayerInputController _playerInputController;
    
    [Space]
    [SerializeField] private Transform _playerBody;
    private Transform _playerCamera;
    
    [Header("Gravity Movement Settings")] 
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private float _runSpeed = 6f;
    [SerializeField] private float _groundSmoothTime = 0.1f;
    [SerializeField] private float _airSmoothTime = 0.5f;
    
    [Header("Weightless Movement Settings")]
    [SerializeField] private float _thrustAcceleration = 0.2f;
    [SerializeField] private float _maxThrust = 2f;
    [SerializeField] private float _weightlessRotationSpeed = 1f;    
    [SerializeField] private float _weightlessSmoothingAmount = 0.05f;
    
    [Header("Jump Settings")]
    [SerializeField] private float _jumpForce = 20f; 
    [SerializeField] private float _stickToGroundForce = 8f;

    //===== Gravity Movement Variables =====
    private Vector3 _movementVector;
    private Vector3 _smoothRef;
    
    //===== Weightless Movement Variables =====
    private Vector2 _weightlessRotationInput;
    private Vector2 _weightlessSmoothRef;
    
    //===== Movement States =====
    private Action CurrentMovement;
    private bool _isThrusting = false;

    #region Subscribe
    
    private void OnEnable()
    {
        _playerInputController.OnSwitchInputState += OnSwitchInputState;
        _playerInputController.OnJump += OnJump;
    }
    
    private void OnDisable()
    {
        _playerInputController.OnSwitchInputState -= OnSwitchInputState;
        _playerInputController.OnJump -= OnJump;
    }
    
    #endregion

    protected new void Awake()
    {
        base.Awake();
        _playerCamera = _playerBody.GetComponentInChildren<Camera>().transform;
        CurrentMovement = GravityMovement;
    }
    
    #region Event Methods
    
    private void OnJump(bool isPressed)
    {
        if (isPressed)
        {
            _isThrusting = true;
            if (!_playerInputController.IsGrounded) return;
            _rb.AddForce(transform.up * _jumpForce, ForceMode.VelocityChange);
        }
        else
        {
            _isThrusting = false;
        }
    }
    
    private void OnSwitchInputState(PlayerInputController.MovementType newState)
    {
        CurrentMovement = newState switch
        {
            PlayerInputController.MovementType.Gravity => GravityMovement,
            PlayerInputController.MovementType.Weightless => WeightlessMovement,
            _ => throw new ArgumentOutOfRangeException(nameof(newState), newState, null)
        };
    }

    #endregion

    //===== Movement States =====
    
    private void Update()
    {
        if (_playerInputController.IsUpdateLocked) return;
        
        CurrentMovement();
    }
    
    private void GravityMovement()
    {
        //stick to ground force
        if (_playerInputController.IsGrounded && !_isThrusting) _rb.AddForce(-transform.up * _stickToGroundForce, ForceMode.VelocityChange); 
        
        float currentSpeed = _playerInputController.IsRunning ? _runSpeed : _walkSpeed;
        Vector3 targetVelocity = _playerBody.TransformDirection(_playerInputController.MovementInput.normalized) * currentSpeed;
        float smoothTime = (_playerInputController.IsGrounded) ? _groundSmoothTime : _airSmoothTime;
        _movementVector = Vector3.SmoothDamp(_movementVector, targetVelocity, ref _smoothRef, smoothTime);
    }
    
    private void WeightlessMovement()
    {
        Vector3 inputDirection = _playerInputController.MovementInput.normalized;
        
        _weightlessRotationInput.x = Mathf.SmoothDamp(_weightlessRotationInput.x, inputDirection.x, ref _weightlessSmoothRef.x, _weightlessSmoothingAmount);
        _weightlessRotationInput.y = Mathf.SmoothDamp(_weightlessRotationInput.y, inputDirection.z, ref _weightlessSmoothRef.y, _weightlessSmoothingAmount);
        _weightlessRotationInput *= _weightlessRotationSpeed;
        
        Vector3 pitchAxis = _playerCamera.right;
        Vector3 rollAxis  = -_playerCamera.forward;

        transform.Rotate(pitchAxis, _weightlessRotationInput.y, Space.World);
        transform.Rotate(rollAxis, _weightlessRotationInput.x, Space.World);
        
        //===== Thrusters

        if (_isThrusting)
        {
            _movementVector += _playerCamera.forward * _thrustAcceleration * Time.deltaTime;
            _movementVector = Vector3.ClampMagnitude(_movementVector, _maxThrust);
        }
    }

    //===== Gravity Applications =====
    
    protected new void FixedUpdate()
    {
        if (_playerInputController.IsUpdateLocked) return;
        
        base.FixedUpdate();
        transform.position += _movementVector * Time.fixedDeltaTime;
    }
    
    protected override void ApplySourceGravity()
    {
        GetClosestSourceToObject(out Vector3 strongestPull, out float? distanceToSurface);
        
        if (strongestPull.sqrMagnitude < _weakestGravityStrength || !distanceToSurface.HasValue)
        {
            _playerInputController.CurrentMovementType = PlayerInputController.MovementType.Weightless;
        }
        else
        {
            _playerInputController.CurrentMovementType = PlayerInputController.MovementType.Gravity;
            RotateObjectToSourceUp(strongestPull, distanceToSurface);
        }
    }

    protected override void ApplyDirectionalGravity()
    {
        _playerInputController.CurrentMovementType = PlayerInputController.MovementType.Gravity;
        base.ApplyDirectionalGravity();
    }
    
    //===== External Callers =====
    //methods that are called by other scripts to modify input/position

    public PlayerMovement ZeroOutMovementVector()
    {
        _movementVector = Vector3.zero;
        _rb.angularVelocity = _movementVector;
        _rb.linearVelocity = _movementVector;
        return this;
    } 

}
