using System;
using UnityEngine;

public class ShipMovement : MonoBehaviour
    , ICallbacks
{
    [SerializeField] private ShipInputController _shipInputController;
    [SerializeField] private CameraControlInteractable _shipInteractable;
    [SerializeField] private GravityDirectionalTrigger _shipGravityTrigger;
    
    [Header("Camera Settings")]
    [SerializeField] private float _rotateSpeed;
    
    [Header("Movement Settings")]
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _acceleration;

    public event Action<Vector3> OnShipMove; 

    private Vector3 _movementVector;
    private Vector3 _snapShotUp = Vector3.up;
    private Vector3 _snapShotRight = Vector3.right;

    private Transform _engineCache;

    #region Subscribe

    private void OnEnable()
    {
        _shipInteractable.Delegate = this;
        _shipInputController.OnEndControlShip += OnEndControlShip;
    }

    private void OnDisable()
    {
        _shipInteractable.Delegate = null;
        _shipInputController.OnEndControlShip -= OnEndControlShip;
    }

    #endregion

    private void Start()
    {
        _engineCache = InputEngine.Engine;
    }
    
    //===== Ship Camera Animation =====

    public void OnAnimateComplete()
    {
        _snapShotUp = _engineCache.up;
        _snapShotRight = _engineCache.right;
        InputEngine.SwitchActionMap(InputMapType.Ship);
    }

    private void OnEndControlShip()
    {
        _movementVector = Vector3.zero; //maybe lerp to slow instead of stopping immediately 

        _shipGravityTrigger.SetGravityToUp();
        _shipGravityTrigger.OnGravityBodyEnter(InputEngine.GetPlayerController().Input_PlayerMovement);
        _shipInteractable.AnimateToOrigin(() => { InputEngine.SwitchActionMap(InputMapType.Player); });
    }

    //===== Movement =====
    
    private void Update()
    {
        if (_shipInputController.IsUpdateLocked) return;
        
        Vector2 mouseDelta = _shipInputController.MouseDelta;
        
        Quaternion yawDelta = Quaternion.AngleAxis(mouseDelta.x * _rotateSpeed, _engineCache.rotation * Vector3.up);
        Quaternion pitchDelta = Quaternion.AngleAxis(-mouseDelta.y * _rotateSpeed, _engineCache.rotation * Vector3.right);
        
        _engineCache.rotation = Quaternion.Normalize(yawDelta * pitchDelta * _engineCache.rotation);
    }
    
    private void FixedUpdate()
    {
        if (_shipInputController.IsUpdateLocked) return;

        Vector3 relativeDirection = _engineCache.TransformDirection(_shipInputController.MovementInput.normalized);
        _movementVector += relativeDirection * _acceleration;
        _movementVector = Vector3.ClampMagnitude(_movementVector, _maxSpeed);
        _engineCache.position += _movementVector * Time.fixedDeltaTime;
    }

    private void LateUpdate()
    {
        if (_shipInputController.IsUpdateLocked) return;
        
        OnShipMove?.Invoke(_engineCache.position);
    }

    /// <summary>
    /// Reset the ship position to the world origin 
    /// </summary>
    public void ResetToWorldOrigin()
    {
        ResetToPosition(Universe.WorldOrigin);
    }

    /// <summary>
    /// Set the ship position to the provided vector
    /// </summary>
    public void ResetToPosition(Vector3 pos)
    {
        _engineCache.position = pos + (_engineCache.position.normalized);
    }
}
