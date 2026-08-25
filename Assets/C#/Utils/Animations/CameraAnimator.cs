using System;
using ITween;
using ITween.Animator;
using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Take control of the main camera and move it to a certain spot for a time
/// </summary>
public class CameraAnimator : MonoBehaviour
{
    [SerializeField] protected Transform _targetSpot;
    [SerializeField] private TweenSettings_Simple_Flagless _settings;
    
    [Header("Camera Settings")]
    
    [SerializeField] private bool _overrideMainCamera;
    [Tooltip("Camera to process interactions from, leave empty to use main camera")]
    [SerializeField, ShowIf(nameof(_overrideMainCamera))] private Camera _camera = null;
    
    [SerializeField] private bool _overrideReturnPosition;
    [Tooltip("Position to return camera to when done, leave empty to use global camera reference")]
    [SerializeField, ShowIf(nameof(_overrideReturnPosition))] private Transform _returnPosition = null;
    
    [Header("Settings")]
    [SerializeField] private bool _lockInput = true;
    
    private void Start()
    {
        if (_camera == null) _camera = Camera.main;
    }

    public virtual void AnimateToTarget(Action onComplete = null)
    {
        Tween t = _camera.transform.IT_Move(_targetSpot, _settings);
        
        InputEngine.HasInputLock = _lockInput;
        t.Start(() =>
        {
            InputEngine.HasInputLock = false;
            onComplete?.Invoke();
        });
    }

    public virtual void AnimateToOrigin(Action onComplete = null)
    {
        Tween t = _overrideReturnPosition ? 
            _camera.transform.IT_Move(_returnPosition, _settings) : _camera.transform.IT_Move(InputEngine.CameraOriginRef, _settings);
        
        InputEngine.HasInputLock = _lockInput;
        t.Start(() =>
        {
            InputEngine.GetPlayerController().Input_PlayerCamera.RecenterView();
            InputEngine.HasInputLock = false;
            onComplete?.Invoke();
        });
    }
}
