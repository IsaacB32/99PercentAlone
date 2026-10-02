using System;
using Isaac.Extensions;
using ITween;
using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Lock input, take control of the main camera and move it to a target spot
/// </summary>
public class CameraAnimator : MonoBehaviour
{
    [SerializeField, Required("Camera target is required")] protected Transform _cameraTargetPoint;
    [SerializeField] private TweenSettings_Simple_Flagless _settings;
    
    [Space(5)]
    [SerializeField] private bool _overrideMainCamera;
    [Tooltip("Camera to process interactions from, leave empty to use main camera")]
    [SerializeField, Indent, ShowIf(nameof(_overrideMainCamera))] private Camera _camera = null;
    
    [SerializeField] private bool _overrideReturnPosition;
    [Tooltip("Position to return camera to when done, leave empty to use global camera reference")]
    [SerializeField, Indent, ShowIf(nameof(_overrideReturnPosition))] private Transform _returnPosition = null;
    
    [Space(5)]
    [SerializeField] private bool _lockInput = true;
    
    private void Start()
    {
        if (_camera == null) _camera = Camera.main;
    }

    public virtual void AnimateToTarget(bool animate = true, Action onComplete = null)
    {
        if (!animate)
        {
            onComplete?.Invoke();
            return;
        }
        
        Tween t = _camera.transform.IT_Move(_cameraTargetPoint, _settings);
        
        InputEngine.GetPlayerController().ResetValues();
        if (_lockInput) InputEngine.InputLock.RegisterLockHolder(this);
        t.Start(() =>
        {
            if (_lockInput) InputEngine.InputLock.UnregisterLockHolder(this);
            onComplete?.Invoke();
        });
    }

    public virtual void AnimateToOrigin(bool animate = true, Action onComplete = null)
    {
        if (!animate)
        {
            onComplete?.Invoke();
            return;
        }
        
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
    
#if UNITY_EDITOR
    //===== Editor Accessor =====

    public Transform Target => _cameraTargetPoint;
    public static bool IsPreview { get; set; } = false;
    public static TransformSnapshot Cache { get; set; }
#endif
}
