using System;
using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Handles interactions from the main camera with a raycast 
/// </summary>
public class Interactor : MonoBehaviour
{
    [Header("Controller")]
    [SerializeField] private PlayerInputController _playerInputController;
    [Space]
    
    [SerializeField] private bool _overrideMainCamera;
    [Tooltip("Camera to process interactions from, leave empty to use main camera")]
    [SerializeField, ShowIf(nameof(_overrideMainCamera))] private Camera _camera = null;

    [SerializeField] private LayerMask _interactionLayerMask = Layers.Interaction;
    [SerializeField] private float _interactionDistance = 5f;
    
    [Space]
    [SerializeField] private bool _drawLook = false;

    private Vector2 _mousePos;
    
    private void Awake()
    {
        if (_camera == null) _camera = Camera.main;
    }
    
    private void LateUpdate()
    {
        Vector3 worldOrigin = _camera.ScreenToWorldPoint(_playerInputController.MousePosition);
        if (Physics.Raycast(worldOrigin, transform.forward, out RaycastHit hit, _interactionDistance, _interactionLayerMask))
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();
            Interactable.RefreshHovering(interactable);
            
            if (_playerInputController.InteractPressedThisFrame) interactable.Select();
        } 
        else Interactable.RefreshHovering(null);
    }

    #if UNITY_EDITOR
    
    private void OnDrawGizmos()
    {
        if (!_drawLook || !Application.isPlaying) return;
        Vector3 worldOrigin = _camera.ScreenToWorldPoint(_playerInputController.MousePosition);
        Gizmos.DrawRay(worldOrigin, transform.forward * _interactionDistance);
    }
    
    #endif
}
