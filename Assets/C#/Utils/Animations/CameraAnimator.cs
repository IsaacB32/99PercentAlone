using System;
using ITween;
using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Take control of the main camera and move it to a certain spot for a time when interacted
/// </summary>
public class CameraAnimator : MonoBehaviour
{
    [SerializeField] protected Transform _cameraTargetPoint;
    [SerializeField] private TweenSettings_Simple_Flagless _settings;
    
    [Space(5)]
    [SerializeField] private bool _overrideMainCamera;
    [Tooltip("Camera to process interactions from, leave empty to use main camera")]
    [SerializeField, Intent, ShowIf(nameof(_overrideMainCamera))] private Camera _camera = null;
    
    [SerializeField] private bool _overrideReturnPosition;
    [Tooltip("Position to return camera to when done, leave empty to use global camera reference")]
    [SerializeField, Intent, ShowIf(nameof(_overrideReturnPosition))] private Transform _returnPosition = null;
    
    [Space(5)]
    [SerializeField] private bool _lockInput = true;
    
    private void Start()
    {
        if (_camera == null) _camera = Camera.main;
    }

    public virtual void AnimateToTarget(Action onComplete = null)
    {
        Tween t = _camera.transform.IT_Move(_cameraTargetPoint, _settings);
        
        if (_lockInput) InputEngine.InputLock.RegisterLockHolder(this);
        t.Start(() =>
        {
            if (_lockInput) InputEngine.InputLock.UnregisterLockHolder(this);
            onComplete?.Invoke();
        });
    }

    public virtual void AnimateToOrigin(Action onComplete = null)
    {
        Tween t = _overrideReturnPosition ? 
            _camera.transform.IT_Move(_returnPosition, _settings) : _camera.transform.IT_Move(InputEngine.CameraOriginRef, _settings);
        
        if (_lockInput) InputEngine.InputLock.RegisterLockHolder(this);
        t.Start(() =>
        {
            InputEngine.GetPlayerController().Input_PlayerCamera.RecenterView();
            if (_lockInput) InputEngine.InputLock.UnregisterLockHolder(this);
            onComplete?.Invoke();
        });
    }
}
