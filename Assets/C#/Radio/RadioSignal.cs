using UnityEngine;

public class RadioSignal
{
   public UniverseCoordinates Origin { get; private set; }

   public RadioSignal(UniverseCoordinates origin)
   {
      Origin = origin;
   }
}
