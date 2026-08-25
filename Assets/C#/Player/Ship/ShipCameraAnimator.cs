using System;
using ITween;
using UnityEngine;

/// <summary>
/// Animate the camera to and from the ship controls 
/// </summary>
public class ShipCameraAnimator : CameraAnimator
{
   [Header("Ship Settings")] 
   [SerializeField, Tooltip("where will the player be when steering the ship")] private Transform _playerPoint;

   public override void AnimateToTarget(Action onComplete = null)
   {
      InputEngine.RemoveInput();
      base.AnimateToTarget(() =>
      {
         InputEngine.SwitchActionMap(InputMapType.Ship);
         InputEngine.GetPlayerController().SnapPlayerPosition(_playerPoint.position, cameraMove: false);
         InputEngine.GetPlayerController().Input_PlayerCamera.RecenterView(_targetSpot.forward);
      });
   }
}
