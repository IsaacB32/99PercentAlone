
/// <summary>
/// Animate the camera to and from the ship controls 
/// </summary>
public class ShipCameraInteractable : CameraControlInteractable, IInteractable
{
   protected override void BeforeAnimate()
   {
      InputEngine.RemoveInput();
   }

   protected override void AfterAnimate()
   {
      InputEngine.SwitchActionMap(InputMapType.Ship);
   }
}
