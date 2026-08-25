using System;
using UnityEngine;

/// <summary>
/// Animates the camera and moves the player to a target point when finished
/// </summary>
public class CameraControlInteractable : CameraAnimator, IInteractable
{
    [SerializeField, Tooltip("where will the player be when the animation finishes")] private Transform _playerPoint;

    public void OnSelect()
    {
        AnimateToTarget();
    }
    
    public override void AnimateToTarget(Action onComplete = null)
    {
        BeforeAnimate();
        base.AnimateToTarget(() =>
        {
            AfterAnimate();
            InputEngine.GetPlayerController().SnapPlayerPosition(_playerPoint.position, cameraMove: false);
            InputEngine.GetPlayerController().Input_PlayerCamera.RecenterView(_targetSpot.forward);
            onComplete?.Invoke();
        });
    }

    /// <summary>
    /// Callback that gets called right before the animation is started
    /// </summary>
    protected virtual void BeforeAnimate() {}
    
    /// <summary>
    /// Callback that gets called immediately upon finishing the animation
    /// </summary>
    protected virtual void AfterAnimate() {}
}
