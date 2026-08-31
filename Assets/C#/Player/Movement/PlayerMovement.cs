using System;
using ITween;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerMovement : GravityBody
{
    public const float COLLISION_DISTANCE = 0.8277778F; 
    
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

    //===== Exposed Properties =====
    public Rigidbody RB => _rb;
    
    //===== Gravity Movement Variables =====
    private Vector3 _movementVector;
    private Vector3 _smoothRef;
    
    //===== Weightless Movement Variables =====
    private Vector2 _weightlessRotationInput;
    private Vector2 _weightlessSmoothRef;
    private Vector2 _rotationVector;
    
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
        switch (newState)
        {
            case PlayerInputController.MovementType.Gravity:
                RB.interpolation = RigidbodyInterpolation.Interpolate;
                CurrentMovement = GravityMovement;
                break;
            case PlayerInputController.MovementType.Weightless:
                RB.interpolation = RigidbodyInterpolation.None;
                CurrentMovement = WeightlessMovement;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
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
        
        _rotationVector.x = Mathf.SmoothDamp(_rotationVector.x, inputDirection.x, ref _weightlessSmoothRef.x, _weightlessSmoothingAmount);
        _rotationVector.y = Mathf.SmoothDamp(_rotationVector.y, inputDirection.z, ref _weightlessSmoothRef.y, _weightlessSmoothingAmount);

        float rotationSpeed = _weightlessRotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.right, _rotationVector.y * rotationSpeed, Space.Self);
        transform.Rotate(Vector3.forward, -_rotationVector.x * rotationSpeed, Space.Self);
        
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

        //===== Collision Smoothing
        if (_movementVector.magnitude > 0f)
        {
            if (_rb.SweepTest(_movementVector.normalized, out RaycastHit hit, COLLISION_DISTANCE, QueryTriggerInteraction.Ignore))
            {
                float distance = hit.distance;
                float percent = Easing.EaseOutQuad(distance / COLLISION_DISTANCE);
                _movementVector *= percent;
            }
        }
        PhysicsUpdate();
    }

    private void PhysicsUpdate()
    {
        base.FixedUpdate();
        _rb.MovePosition(_rb.position + _movementVector * Time.fixedDeltaTime);
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

    /// <summary>
    /// Set _movementVector to zero and stop rigidbody velocity
    /// </summary>
    public PlayerMovement ZeroOutMovementVector()
    {
        _movementVector = Vector3.zero;
        _rb.angularVelocity = _movementVector;
        _rb.linearVelocity = _movementVector;
        return this;
    }

    /// <summary>
    /// Set the physics of the Player to on/off
    /// </summary>
    public PlayerMovement SetEnablePhysics(bool value)
    {
        if (value) //enable
        {
            _rb.isKinematic = false;
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
        else //disable 
        {
            _rb.isKinematic = true;
            _rb.constraints = RigidbodyConstraints.FreezeAll;
            _rb.interpolation = RigidbodyInterpolation.None;
        }
        return this;
    }

    /// <summary>
    /// Force Update the physics for a certain number of frames
    /// </summary>
    /// <param name="amount">amount of frames to update for</param>
    /// <param name="onComplete">action once updates are complete</param>
    public PlayerMovement ForceStepPhysics(int amount, [NotNull] Action onComplete)
    {
        if (amount == 0)
        {
            onComplete.Invoke();
            return this;
        }
        
        Delay.WaitForNextFrame(amount, () =>
        {
            CurrentMovement();
            PhysicsUpdate();
        }, onComplete);
        
        return this;
    }

}
