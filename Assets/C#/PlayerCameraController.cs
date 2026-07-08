using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Camera Constraints")] 
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float _minVerticalAngle = -90f;
    [SerializeField] private float _maxVerticalAngle = 90f;

    [Header("Smoothing")]
    [Tooltip("Lower = more responsive. 0.02-0.04 feels snappy, 0.1+ feels floaty/laggy.")]
    [SerializeField] private float _smoothTime = 0.02f;
    [SerializeField] private float _spaceSmoothTime = 0.012f;

    [Header("Zoom/Aim Settings")] 
    [SerializeField] private float _normalFOV = 60f;
    [SerializeField] private float _zoomedFOV = 30f;
    [SerializeField] private float _runningFOV = 70f;

    [Tooltip("Higher = faster FOV transitions. Frame-rate independent.")] 
    [SerializeField] private float _zoomSpeed = 10f;
    [Range(0f, 1f)] [SerializeField] private float _aimShakeMultiplier = 0.3f;

    [Header("Headbob Settings")] 
    [SerializeField] private bool _enableCameraShake = true;
    [Tooltip("Vertical bob amplitude when walking (meters).")] 
    [SerializeField] private float _walkBobAmount = 0.04f;
    [Tooltip("Vertical bob amplitude when running (meters).")] 
    [SerializeField] private float _runBobAmount = 0.07f;
    [Tooltip("Steps per second when walking.")] 
    [SerializeField] private float _walkBobFrequency = 1.8f;
    [Tooltip("Steps per second when running.")] 
    [SerializeField] private float _runBobFrequency = 2.6f;
    [Tooltip("Horizontal sway amplitude as a fraction of the vertical bob.")] [Range(0f, 1f)] 
    [SerializeField] private float _bobHorizontalRatio = 0.5f;
    [Tooltip("How quickly the bob amplitude fades in/out when starting/stopping.")] 
    [SerializeField] private float _bobSmoothing = 10f;

    [Header("Landing Shake Settings")] 
    [SerializeField] private float _landingShakeIntensity = 0.15f;
    [SerializeField] private float _landingShakeDuration = 0.3f;
    [SerializeField] private float _landingShakeFrequency = 25f;

    //===== References =====
    private Camera _playerCamera;
    private Transform _playerBody;
    private Transform _parentObject;
    
    //===== Mouse Look =====
    private float _xRotation, _yRotation;
    private float _smoothXRot, _smoothYRot;
    private float _smoothXRef, _smoothYRef; //these are useless values (for SmoothDamp)

    //===== Zoom variables =====
    private float _targetFOV;
    private float _currentFOV;

    //===== Camera shake / head-bob variables =====
    private Vector3 _cameraShakeOffset;
    private float _bobPhase; // accumulated step phase in radians
    private float _currentBobAmount; // smoothed amplitude
    private float _currentBobFreq; // smoothed step frequency
    private float _landingShakeTimer;
    private float _landingShakeSeed;
    private bool _wasGrounded = true;
    
    //===== Movement States =====
    private Action<Vector2> CurrentMovement;

    private void Awake()
    {
        _playerCamera = GetComponent<Camera>();
        _playerBody = transform.parent;
        _parentObject = _playerBody.parent;

        CurrentMovement = WeightlessMovement;
    }

    private void Start()
    {
        // Lock cursor to center of screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _normalFOV = _playerCamera.fieldOfView;
        _currentFOV = _normalFOV;
        _targetFOV = _normalFOV;
    }

    #region Subscribe

    private void OnEnable()
    {
        InputCatcher.OnLook += OnLook;
        InputCatcher.OnSwitchInputState += OnSwitchInputState;

    }

    private void OnDisable()
    {
        InputCatcher.OnLook -= OnLook;
        InputCatcher.OnSwitchInputState -= OnSwitchInputState;
    }

    #endregion
    
    #region Event Methods
    
    private void OnLook(Vector2 lookVector)
    {
        _xRotation += lookVector.x * mouseSensitivity;
        _yRotation -= lookVector.y * mouseSensitivity;
        CurrentMovement(lookVector);
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
    
    private void GravityMovement(Vector2 lookVector)
    {
        _yRotation = Mathf.Clamp(_yRotation - lookVector.y * mouseSensitivity, _minVerticalAngle, _maxVerticalAngle);
        
        _smoothYRot = Mathf.SmoothDampAngle(_smoothYRot, _yRotation, ref _smoothYRef, _smoothTime);
        float smoothXOld = _smoothXRot;
        _smoothXRot = Mathf.SmoothDampAngle(_smoothXRot, _xRotation, ref _smoothXRef, _smoothTime);
        
        transform.localEulerAngles = Vector3.right * _smoothYRot;
        _playerBody.Rotate(Vector3.up * Mathf.DeltaAngle(smoothXOld, _smoothXRot), Space.Self);
    }

    private void WeightlessMovement(Vector2 lookVector)
    {
        float smoothYOld = _smoothYRot;
        _smoothYRot = Mathf.SmoothDampAngle(_smoothYRot, _yRotation, ref _smoothYRef, _spaceSmoothTime);
        float smoothXOld = _smoothXRot;
        _smoothXRot = Mathf.SmoothDampAngle(_smoothXRot, _xRotation, ref _smoothXRef, _spaceSmoothTime);
        
        Vector3 rotationVector = new Vector3(Mathf.DeltaAngle (smoothYOld, _smoothYRot), Mathf.DeltaAngle (smoothXOld, _smoothXRot));
        _parentObject.Rotate(rotationVector, Space.Self);
    }

    private void MenuMovement(Vector2 lookVector)
    {
        throw new NotImplementedException();
    }
    
    #endregion
    
    void Update()
    {
        HandleZoom();
        HandleCameraShake();
    }
    
    private void HandleZoom()
    {
        bool isPlayerRunning = InputCatcher.IsRunning;

        if (InputCatcher.IsAiming) _targetFOV = _zoomedFOV;
        else if (InputCatcher.CurrentInputState == PlayerInputState.Gravity && isPlayerRunning) _targetFOV = _runningFOV;
        else  _targetFOV = _normalFOV;

        float zoomT = 1f - Mathf.Exp(-_zoomSpeed * Time.deltaTime);
        _currentFOV = Mathf.Lerp(_currentFOV, _targetFOV, zoomT);

        if (_playerCamera != null)  _playerCamera.fieldOfView = _currentFOV;
    }

    private void HandleCameraShake()
    {
        if (!_enableCameraShake)
        {
            _cameraShakeOffset = Vector3.MoveTowards(_cameraShakeOffset, Vector3.zero, Time.deltaTime);
            return;
        }

        bool isGrounded = InputCatcher.IsGrounded;
        bool isMoving = InputCatcher.IsMoving;
        bool isRunning = InputCatcher.IsRunning;

        // Detect landing
        if (isGrounded && !_wasGrounded)
        {
            _landingShakeTimer = _landingShakeDuration;
            _landingShakeSeed = Random.value * 100f;
        }
        _wasGrounded = isGrounded;
        
        float targetAmount = 0f;
        float targetFreq = _walkBobFrequency;
        if (isMoving && isGrounded)
        {
            targetAmount = isRunning ? _runBobAmount : _walkBobAmount;
            targetFreq = isRunning ? _runBobFrequency : _walkBobFrequency;
        }

        if (InputCatcher.IsAiming)
        {
            targetAmount *= _aimShakeMultiplier;
        }

        float t = 1f - Mathf.Exp(-_bobSmoothing * Time.deltaTime);
        _currentBobAmount = Mathf.Lerp(_currentBobAmount, targetAmount, t);
        _currentBobFreq = Mathf.Lerp(_currentBobFreq, targetFreq, t);
        
        if (_currentBobAmount > 0.0001f)
        {
            _bobPhase += _currentBobFreq * 2f * Mathf.PI * Time.deltaTime;
            if (_bobPhase > Mathf.PI * 2f) _bobPhase -= Mathf.PI * 2f;
        }
        else
        {
            _bobPhase = Mathf.LerpAngle(_bobPhase * Mathf.Rad2Deg, 0f, t) * Mathf.Deg2Rad;
        }

        // Vertical bob: full sine. Horizontal sway: half-frequency cosine so it alternates left/right per step.
        float bobY = Mathf.Sin(_bobPhase) * _currentBobAmount;
        float bobX = Mathf.Cos(_bobPhase * 0.5f) * _currentBobAmount * _bobHorizontalRatio;
        Vector3 bobOffset = new Vector3(bobX, bobY, 0f);

        // --- Landing shake (impulse, decays over duration) ---
        Vector3 landingOffset = Vector3.zero;
        if (_landingShakeTimer > 0f)
        {
            _landingShakeTimer -= Time.deltaTime;
            float decay = Mathf.Clamp01(_landingShakeTimer / _landingShakeDuration);
            float amp = decay * decay * _landingShakeIntensity; // ease-out
            float n = Time.time * _landingShakeFrequency;
            float lx = (Mathf.PerlinNoise(n, _landingShakeSeed) - 0.5f) * 2f;
            float ly = (Mathf.PerlinNoise(_landingShakeSeed, n) - 0.5f) * 2f;
            landingOffset = new Vector3(lx * amp * 0.5f, ly * amp, 0f);
        }

        _cameraShakeOffset = bobOffset + landingOffset;
    }
}

/*
 
     // [Header("Wall Collision Settings")]
   // [SerializeField] private bool _enableWallCollision = true;
   // [SerializeField] private LayerMask _wallCollisionMask = ~0;
   // [SerializeField] private float _collisionRadius = 0.2f;
   // [SerializeField] private float _collisionBuffer = 0.1f;
   // [SerializeField] private float _collisionSmoothTime = 0.05f;
   // [SerializeField] private float _minDistanceFromPivot = 0.05f;
 
     // Wall collision variables
   private float _currentCollisionDistance;
   private float _collisionDistanceVelocity;
   private Collider[] _ownColliders;
 * private void LateUpdate()
   {
       ApplyCameraPosition();
   }
   
   private void ApplyCameraPosition()
   {
       // if (_playerBody == null)
       // {
       //     transform.localPosition = _baseLocalPosition + _cameraShakeOffset;
       //     return;
       // }
       //
       // Vector3 desiredOffset = _baseLocalPosition + _cameraShakeOffset;
       // float desiredDistance = desiredOffset.magnitude;
       //
       // if (!_enableWallCollision || desiredDistance < Mathf.Epsilon)
       // {
       //     transform.localPosition = desiredOffset;
       //     _currentCollisionDistance = desiredDistance;
       //     return;
       // }
       //
       // Vector3 pivotWorld = _playerBody.position;
       // Vector3 desiredWorld = _playerBody.TransformPoint(desiredOffset);
       // Vector3 dir = (desiredWorld - pivotWorld).normalized;
       //
       // float targetDistance = desiredDistance;
       // RaycastHit[] hits = Physics.SphereCastAll(pivotWorld, _collisionRadius, dir, desiredDistance, _wallCollisionMask, QueryTriggerInteraction.Ignore);
       // float closest = float.PositiveInfinity;
       // foreach (RaycastHit hit in hits)
       // {
       //     if (IsOwnCollider(hit.collider) || hit.distance <= 0f) continue;
       //     if (hit.distance < closest) closest = hit.distance;
       // }
       //
       // if (closest < float.PositiveInfinity)
       // {
       //     targetDistance = Mathf.Max(_minDistanceFromPivot, closest - _collisionBuffer);
       // }
       //
       // _currentCollisionDistance = Mathf.SmoothDamp(_currentCollisionDistance, targetDistance, ref _collisionDistanceVelocity, _collisionSmoothTime);
       // Vector3 finalOffset = desiredOffset.normalized * _currentCollisionDistance;
       // transform.localPosition = finalOffset;
   }

   private bool IsOwnCollider(Collider c)
   {
       if (c == null || _ownColliders == null) return false;
       return _ownColliders.Any(col => col == c);
   }
 */
