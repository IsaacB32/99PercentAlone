using System;
using System.Collections.Generic;
using ITween;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls and switches between different InputActionMaps
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class InputEngine : MonoBehaviour
{
    //=!= SINGLETON MARKER =!=
    private static InputEngine _instance;

    [SerializeField] private InputMapType _defaultInputType;
    
    [Space]
    [SerializeField] private PlayerInputController _playerInputController;
    [SerializeField] private ShipInputController _shipInputController;
    private static InputController _activeController;
    
    [Space]
    [SerializeField] private LayerMask _collisionLayers;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;

    public static LayerMask CollisionLayers => _instance._collisionLayers;
    public static LayerMask GroundLayer => _instance._groundLayer;
    public static LayerMask WallLayer => _instance._wallLayer;
    
    private static PlayerInput _playerInput;
    public static InputMapType ActiveMap { get; private set; }
    public static event Action<InputMapType> OnSwitchInputMap;

    public static Transform Engine { get; private set; }

    private void Awake()
    {
        if (_instance != null) Destroy(gameObject);
        _instance = this;

        Engine = transform;
        _playerInput = GetComponent<PlayerInput>();
        
        //set defaults
        if (_defaultInputType != InputMapType.Player) GetPlayerController().OnExit(InputMapType.None);
        SwitchActionMap(_defaultInputType);
        
#if UNITY_EDITOR
        Application.targetFrameRate = 60;
#endif
    }

    private void Start()
    {
        // Lock cursor to center of screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void SwitchActionMap(InputMapType mapType)
    {
        if (mapType == ActiveMap) return;

        InputController oldController = _activeController;
        switch (mapType)
        {
            case InputMapType.Player:
                ActiveMap = InputMapType.Player;
                _activeController = GetPlayerController();
                break;
            case InputMapType.Ship:
                ActiveMap = InputMapType.Ship;
                _activeController = GetShipController();
                break;
            case InputMapType.Menu:
                ActiveMap = InputMapType.Menu;
                throw new NotImplementedException("Menu controller is not finished");
            case InputMapType.None:
                ActiveMap = InputMapType.None;
                _activeController = null;
                oldController.OnExit(ActiveMap);
                return;
            default:
                ActiveMap = InputMapType.None;
                throw new Exception("InputMapType is set to NONE, please set a type");
        }
        
        oldController?.OnExit(ActiveMap);
        _activeController?.OnEnter(ActiveMap);
        
        OnSwitchInputMap?.Invoke(ActiveMap);
        _playerInput.SwitchCurrentActionMap(ActiveMap.ToString());
    }
    
    /// <summary>
    /// Sets the current input as None to block things from happening while input is switching
    /// </summary>
    public static void RemoveInput() { SwitchActionMap(InputMapType.None); }
    
    //===== Input Lock =====

    private Lock _inputLock = new Lock();
    public static Lock InputLock => _instance._inputLock;

    //===== Control Reference =====
    
    public static PlayerInputController GetPlayerController() { return _instance._playerInputController; }
    public static ShipInputController GetShipController() { return _instance._shipInputController; }

    #region Subreferences

    /// <summary>
    /// Reference to camera origin position for returning camera to default position
    /// </summary>
    public static Transform CameraOriginRef => _instance._playerInputController.CameraOriginReference; 

    #endregion
    
    //===== Lock Cursor (DEBUG) =====
    
    private void Update()
    {
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
}

public enum InputMapType
{
    None,
    Player,
    Ship,
    Menu
}
