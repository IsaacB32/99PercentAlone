using System;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    //===== Gravity =====
    private GravityHost[] _bodies;

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
    [Tooltip("the max speed the camera slerps at when changing movement states")] 
    [SerializeField] private float _gravitySmoothingMax = 3f;
    
    [Header("Jump Settings")]
    [SerializeField] private float _jumpForce = 20f; 
    [SerializeField] private float _stickToGroundForce = 8f;
    
    [Header("Gravity Settings")]
    [SerializeField] private float _weakestGravityStrength = 1f;
    
    //===== References =====
    private Rigidbody _rb;
    
    //===== Gravity Movement Variables =====
    private Vector3 _movementInput;
    private Vector3 _smoothRef;
    
    //===== Weightless Movement Variables =====
    private Vector2 _weightlessRotationInput;
    private Vector2 _weightlessSmoothRef;

    
    //===== Movement States =====
    private Action CurrentMovement;
    private bool _isThrusting = false;
    
    private void Awake()
    {
        _bodies = FindObjectsByType<GravityHost>(); //ToDO
        _rb = GetComponent<Rigidbody>();
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

    #region Movement States

    private void GravityMovement()
    {
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

    #endregion


    private void Update()
    {
        CurrentMovement();
    }

    private void FixedUpdate()
    {
        GravityHost closestBody = null;
        Vector3 strongestGravitationalPull = Vector3.zero;
        foreach (GravityHost body in _bodies)
        {
            Vector3 vectorToCenter = body.VectorToCenter(_rb.position);
            float sqrDst = vectorToCenter.sqrMagnitude;
            Vector3 forceDir = vectorToCenter.normalized;
            Vector3 acceleration = forceDir * GravityHost.GRAVITATIONAL_CONST * body.Mass / sqrDst;
            _rb.AddForce(acceleration, ForceMode.Acceleration);

            if (acceleration.sqrMagnitude > strongestGravitationalPull.sqrMagnitude)
            {
                strongestGravitationalPull = acceleration;
                closestBody = body;
            }
        }
        
        if (strongestGravitationalPull.sqrMagnitude < _weakestGravityStrength)
        {
            InputCatcher.CurrentInputState = PlayerInputState.Weightless;
        }
        else
        {
            InputCatcher.CurrentInputState = PlayerInputState.Gravity;
            Vector3 gravityUp = -strongestGravitationalPull.normalized;

            Quaternion deltaRotation = Quaternion.FromToRotation(transform.up, gravityUp);
            Quaternion targetRotation = deltaRotation * _rb.rotation;
            
            float cameraSmoothSpeed = _gravitySmoothingMax / (1f + closestBody.DistanceToSurface(_rb.position) * 0.1f);
            Quaternion easedRot = Quaternion.Slerp(_rb.rotation, targetRotation, cameraSmoothSpeed * Time.fixedDeltaTime);
            _rb.rotation = easedRot;
        }
        
        _rb.MovePosition(_rb.position + _movementInput * Time.fixedDeltaTime);
    }
}
