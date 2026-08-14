using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerCamera : MonoBehaviour
{
    [Header("Controller")]
    [SerializeField] private PlayerInputController _playerInputController;
    
    [Header("Camera Constraints")] 
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float _minVerticalAngle = -90f;
    [SerializeField] private float _maxVerticalAngle = 90f;

    //Lower = more responsive. 0.02-0.04 feels snappy, 0.1+ feels floaty/laggy
    [Header("Smoothing")]
    [SerializeField] private float _gravitySmoothTime = 0.02f;
    [SerializeField] private float _spaceSmoothTime = 0.012f;
    private float _cameraSmoothingTime;

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
    
    [Header("Collision Settings")]
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private float _collisionRadius = 0.2f;
    [SerializeField] private float _collisionSmoothingTime = 0.05f;
    [SerializeField] private float collisionBuffer = 0.1f;
    [SerializeField] private float minDistanceFromPivot = 0.05f;

    //===== References =====
    private Camera _playerCamera;
    private Transform _playerBody;
    private Transform _parentObject;
    
    //===== Mouse Look =====
    private Vector2 _cameraRotation;
    private Vector2 _cameraSmoothing;
    private Vector2 _smoothingRef; //these are useless values (for SmoothDamp)

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
    
    //===== Wall Collision Settings =====
    private Vector3 _basePosition;
    private float _currentCollisionDistance;
    private float _currentCollisionSmoothRef; //useless SmoothDamp value

    #region Subscribe

    private void OnEnable()
    {
        _playerInputController.OnSwitchInputState += OnSwitchInputState;
    }

    private void OnDisable()
    {
        _playerInputController.OnSwitchInputState -= OnSwitchInputState;
    }

    #endregion

    private void Awake()
    {
        _playerCamera = GetComponent<Camera>();
        _playerBody = transform.parent;
        _parentObject = _playerBody.parent;

        _basePosition = transform.localPosition;
        _currentCollisionDistance = _basePosition.magnitude;
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
    
    #region Event Methods
    
    private void OnSwitchInputState(PlayerInputController.MovementType newState)
    {
        _cameraSmoothingTime = newState switch
        {
            PlayerInputController.MovementType.Gravity => _gravitySmoothTime,
            PlayerInputController.MovementType.Weightless => _spaceSmoothTime,
            _ => throw new ArgumentOutOfRangeException(nameof(newState), newState, null)
        };
    }
    
    #endregion
    
    private void OnLook(Vector2 lookVector)
    {
        _cameraRotation.x += lookVector.x * mouseSensitivity;
        _cameraRotation.y -= lookVector.y * mouseSensitivity;

        CameraMovement(lookVector);
    }
    
    private void CameraMovement(Vector2 lookVector)
    {
        _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookVector.y * mouseSensitivity, _minVerticalAngle, _maxVerticalAngle);
        _cameraSmoothing.y = Mathf.SmoothDampAngle(_cameraSmoothing.y, _cameraRotation.y, ref _smoothingRef.y, _cameraSmoothingTime);
        
        float smoothXOld = _cameraSmoothing.x;
        _cameraSmoothing.x = Mathf.SmoothDampAngle(_cameraSmoothing.x, _cameraRotation.x, ref _smoothingRef.x, _cameraSmoothingTime);
        
        transform.localEulerAngles = Vector3.right * _cameraSmoothing.y;
        _playerBody.Rotate(Vector3.up * Mathf.DeltaAngle(smoothXOld, _cameraSmoothing.x), Space.Self);
    } 
    
    void Update()
    {
        OnLook(_playerInputController.MousePosition);
        
        HandleZoom();
        HandleCameraShake();
        HandleWallCollisions();
    }

    #region Camera Handlers

        private void HandleZoom()
        {
            bool isPlayerRunning = _playerInputController.IsRunning;
    
            if (_playerInputController.IsAiming) _targetFOV = _zoomedFOV;
            else if (_playerInputController.CurrentMovementType == PlayerInputController.MovementType.Gravity && isPlayerRunning) _targetFOV = _runningFOV;
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
    
            bool isGrounded = _playerInputController.IsGrounded;
            bool isMoving = _playerInputController.IsMoving;
            bool isRunning = _playerInputController.IsRunning;
    
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
    
            if (_playerInputController.IsAiming)
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
    
        private void HandleWallCollisions()
        {
            Vector3 desiredOffset = _basePosition + _cameraShakeOffset;
            float desiredDistance = desiredOffset.magnitude;
    
            if (desiredDistance < Mathf.Epsilon)
            {
                transform.localPosition = desiredOffset;
                _currentCollisionDistance = desiredDistance;
                return;
            }
    
            Vector3 pivotWorld = _playerBody.position;
            Vector3 desiredWorld = _playerBody.TransformPoint(desiredOffset);
            Vector3 dir = (desiredWorld - pivotWorld).normalized;
    
            float targetDistance = desiredDistance;
            RaycastHit[] hits = Physics.SphereCastAll(pivotWorld, _collisionRadius, dir, desiredDistance, _collisionMask, QueryTriggerInteraction.Ignore);
            float closest = float.PositiveInfinity;
            foreach (RaycastHit hit in hits)
            {
                if (hit.distance <= 0f) continue;
                if (hit.distance < closest) closest = hit.distance;
            }
            if (closest < float.PositiveInfinity)
            {
                targetDistance = Mathf.Max(minDistanceFromPivot, closest - collisionBuffer);
            }
            
            _currentCollisionDistance = Mathf.SmoothDamp(_currentCollisionDistance, targetDistance, ref _currentCollisionSmoothRef, _collisionSmoothingTime);
    
            // Scale the desired offset to the clamped distance (preserves direction in local space)
            Vector3 finalOffset = desiredOffset.normalized * _currentCollisionDistance;
            transform.localPosition = finalOffset;
        }

    #endregion

}