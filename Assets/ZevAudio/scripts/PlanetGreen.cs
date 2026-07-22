using UnityEngine;

{
    /// <summary>
    /// This class produces audio for various states of the vehicle's movement.
    /// </summary>
    public class ArcadeEngineAudio : MonoBehaviour
{
    public float minRPM = 0;
    public float maxRPM = 5000;
    ArcadeKart arcadeKart;

    private FMODUnity.StudioEventEmitter emitter;

    void Awake()
    {
        arcadeKart = GetComponentInParent<ArcadeKart>();
        emitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    void Update()
    {
        float kartSpeed = arcadeKart != null ? arcadeKart.LocalSpeed() : 0;
        // set RPM value for the FMOD event
        float effectiveRPM = Mathf.Lerp(minRPM, maxRPM, kartSpeed);
        if (emitter != null)
        {
            emitter.SetParameter("RPM", effectiveRPM);
        }
    }
}
}