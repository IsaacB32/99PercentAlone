using UnityEngine;

public abstract class InputController : MonoBehaviour
{
    //===== State Machine =====

    public bool IsUpdateLocked { get; protected set; }
    public abstract void OnEnter(InputMapType oldType);
    public abstract void OnExit(InputMapType newType);
}