using System;
using UnityEditor;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    //===== Gravity =====
    public const float GRAVITATIONAL_CONST = 0.01f;
    private GravityHost[] _bodies;

    [SerializeField] private Transform _playerBody;
    private Transform _playerCamera;
    
    [Header("Gravity Movement Settings")] 
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private float _runSpeed = 6f;
    [SerializeField] private float _groundSmoothTime = 0.1f;
    [SerializeField] private float _airSmoothTime = 0.5f;
    
    [Header("Weightless Movement Settings")]
    [SerializeField] private float _maxThrust = 2f;
    [SerializeField] private float _thrustAcceleration = 0.2f;
    [SerializeField] private float _cameraRotateSpeedToGravity = 1f;
    
    [Header("Jump Settings")]
    [SerializeField] private float _jumpForce = 20f; 
    [SerializeField] private float _stickToGroundForce = 8f;
    
    [Header("Gravity Settings")]
    [SerializeField] private float _weakestGravityStrength = 1f;
    
    //===== References =====
    private Rigidbody _rb;
    
    //===== Variables =====
    private bool _isGrounded;
    private Vector3 _movementInput;
    private Vector3 _smoothRef;
    
    //===== Movement States =====
    private Action CurrentMovement;
    
    private void Awake()
    {
        _bodies = FindObjectsByType<GravityHost>(); //ToDO
        _rb = GetComponent<Rigidbody>();
        _playerCamera = _playerBody.GetComponentInChildren<Camera>().transform;

        CurrentMovement = WeightlessMovement;
    }

    #region Subscribe

        private void OnEnable()
        {
            InputCatcher.OnJump += OnJump;
            InputCatcher.OnSwitchInputState += OnSwitchInputState;
        }
    
        private void OnDisable()
        {
            InputCatcher.OnJump -= OnJump;
            InputCatcher.OnSwitchInputState -= OnSwitchInputState;
        }

    #endregion

    #region Event Methods

    private void OnJump()
    {
        if (!InputCatcher.IsGrounded) return;
        _rb.AddForce(transform.up * _jumpForce, ForceMode.VelocityChange);
        Debug.Log("jumped");
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
        Vector3 targetVelocity = transform.TransformDirection(InputCatcher.MovementVector.normalized);
        _movementInput += targetVelocity * _thrustAcceleration * Time.deltaTime;
        _movementInput = Vector3.ClampMagnitude(_movementInput, _maxThrust);
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
        Vector3 strongestGravitationalPull = Vector3.zero;
        foreach (GravityHost body in _bodies)
        {
            Vector3 vectorToCenter = body.VectorFromCenter(_rb.position);
            float sqrDst = vectorToCenter.sqrMagnitude;
            Vector3 forceDir = vectorToCenter.normalized;
            Vector3 acceleration = forceDir * GRAVITATIONAL_CONST * body.Mass / sqrDst;
            _rb.AddForce(acceleration, ForceMode.Acceleration);

            if (acceleration.sqrMagnitude > strongestGravitationalPull.sqrMagnitude)
            {
                strongestGravitationalPull = acceleration;
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

            if (InputCatcher.IsGrounded) _rb.rotation = targetRotation;
            else
            {
                Quaternion easedRot = Quaternion.Slerp(_rb.rotation, targetRotation, _cameraRotateSpeedToGravity * Time.deltaTime);
                _rb.rotation = easedRot;
            }
           
            //tODO
            //_cameraRotateSpeedToGravity needs to be based on math
            //distance to surface, speed of approaching player, maybe mass/gravity too

        }

        
        _rb.MovePosition(_rb.position + _movementInput * Time.fixedDeltaTime);
    }
}
