using System;
using UnityEngine;

/// <summary>
/// Animates the camera and moves the player to a target point when finished
/// </summary>
public class CameraControlInteractable : CameraAnimator
    , IInteractable
{
    [SerializeField, Tooltip("where will the player be when the animation finishes")] private Transform _playerTargetPoint;
    
    public ICallbacks Delegate { get; set; }
    
    public void OnSelect()
    {
        AnimateToTarget();
    }
    
    public override void AnimateToTarget(Action _ = null)
    {
        if (Delegate == null) throw new NullReferenceException($"Delegate cannot be null, looking for {typeof(ICallbacks)}");
        
        Delegate.OnBeforeAnimate();
        base.AnimateToTarget(() =>
        {
            Delegate.OnAfterAnimate();
            InputEngine.GetPlayerController().SnapPlayerPosition(_playerTargetPoint.position, cameraMove: false);
            InputEngine.GetPlayerController().Input_PlayerCamera.RecenterView(_cameraTargetPoint.forward);
            Delegate.OnAnimateComplete();
        });
    }
}
