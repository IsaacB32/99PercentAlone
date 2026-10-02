using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTools : MonoBehaviour
{
    [Header("Controller")]
    [SerializeField] private PlayerInputController _playerInputController;

    [Header("Tools")]
    [SerializeField] private Tool[] _allTools;

    private bool toggle = true;
    private void Update()
    {
        if (_playerInputController.IsUpdateLocked) return;
        
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            _allTools[0].OperateTool(toggle);
            toggle = !toggle;
        }
    }
}
