using System;
using UnityEngine;

public class PlayerMovementController : GravityBody
{
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
    private Vector3 _movementInput;
    private Vector3 _smoothRef;
    
    //===== Weightless Movement Variables =====
    private Vector2 _weightlessRotationInput;
    private Vector2 _weightlessSmoothRef;
    
    //===== Movement States =====
    private Action CurrentMovement;
    private bool _isThrusting = false;
    
    protected new void Awake()
    {
        base.Awake();
        _playerCamera = _playerBody.GetComponentInChildren<Camera>().transform;
        CurrentMovement = GravityMovement;
    }

    #region Subscribe

        private void OnEnable()
        {
            InputCatcher.OnJumpPressed += OnJumpPressed;
            InputCatcher.OnJumpReleased += OnJumpReleased;
            InputCatcher.OnSwitchInputState += OnSwitchInputState;
        }
    
        private void OnDisable()
        {
            InputCatcher.OnJumpPressed -= OnJumpPressed;
            InputCatcher.OnJumpReleased -= OnJumpReleased;
            InputCatcher.OnSwitchInputState -= OnSwitchInputState;
        }

    #endregion

    #region Event Methods

    private void OnJumpPressed()
    {
        _isThrusting = true;
        if (!InputCatcher.IsGrounded) return;
        _rb.AddForce(transform.up * _jumpForce, ForceMode.VelocityChange);
    }

    private void OnJumpReleased()
    {
        _isThrusting = false;
    }

    private void OnSwitchInputState(PlayerInputState newState)
    {
        CurrentMovement = newState switch
        {
            PlayerInputState.Gravity => GravityMovement,
            PlayerInputState.Weightless => WeightlessMovement,
            PlayerInputState.Menu => MenuMovement,
            _ => throw new ArgumentOutOfRangeException(nameof(newState), newState, null)
        };
    }

    #endregion

    //===== Movement States =====
    
    private void Update()
    {
        CurrentMovement();
    }
    
    private void GravityMovement()
    {
        //stick to ground force
        if (InputCatcher.IsGrounded && !_isThrusting) _rb.AddForce(-transform.up * _stickToGroundForce, ForceMode.VelocityChange); 
        
        float currentSpeed = InputCatcher.IsRunning ? _runSpeed : _walkSpeed;
        Vector3 targetVelocity = _playerBody.TransformDirection(InputCatcher.MovementVector.normalized) * currentSpeed;
        float smoothTime = (InputCatcher.IsGrounded) ? _groundSmoothTime : _airSmoothTime;
        _movementInput = Vector3.SmoothDamp(_movementInput, targetVelocity, ref _smoothRef, smoothTime);
    }
    
    private void WeightlessMovement()
    {
        Vector3 inputDirection = InputCatcher.MovementVector.normalized;
        
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
            _movementInput += _playerCamera.forward * _thrustAcceleration * Time.deltaTime;
            _movementInput = Vector3.ClampMagnitude(_movementInput, _maxThrust);
        }
    }

    private void MenuMovement()
    {
        throw new NotImplementedException();
    }

    //===== Gravity Applications =====
    
    protected new void FixedUpdate()
    {
        base.FixedUpdate();
        _rb.MovePosition(_rb.position + _movementInput * Time.fixedDeltaTime);
    }
    
    protected override void ApplySourceGravity()
    {
        GetClosestSourceToObject(out Vector3 strongestPull, out float? distanceToSurface);
        
        if (strongestPull.sqrMagnitude < _weakestGravityStrength || !distanceToSurface.HasValue)
        {
            InputCatcher.CurrentInputState = PlayerInputState.Weightless;
        }
        else
        {
            InputCatcher.CurrentInputState = PlayerInputState.Gravity;
            RotateObjectToSourceUp(strongestPull, distanceToSurface);
        }
    }

    protected override void ApplyDirectionalGravity()
    {
        InputCatcher.CurrentInputState = PlayerInputState.Gravity;
        base.ApplyDirectionalGravity();
    }
}
