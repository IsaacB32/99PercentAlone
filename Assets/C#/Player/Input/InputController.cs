using UnityEngine;

public abstract class InputController : MonoBehaviour
{
    [SerializeField] private InputMapType _actionMapType;

    protected void SwitchToActionMap()
    {
        InputEngine.SwitchActionMap(_actionMapType);
    }

    //===== State Machine =====

    public bool IsUpdateLocked { get; protected set; }
    public abstract void OnEnter(InputMapType oldType);
    public abstract void OnExit(InputMapType newType);
}