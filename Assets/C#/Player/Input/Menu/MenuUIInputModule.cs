using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Custom UI Input Module to process the virtual cursor for the UI
/// </summary>
public class MenuUIInputModule : PointerInputModule
{
    //=!= This means that regular UI input will no longer work =!=
    
    private const int VIRTUAL_POINTER_ID = -100;
    private PointerEventData _pointerData;

    //===== Properties =====
    public bool IsValid { get; set; } = false;
    public static Vector2 CursorPosition { get; set; }
    
    //===== Input =====
    public static bool IsPressed { get; set; }
    public static bool IsReleased { get; set; }

    public override bool ShouldActivateModule() => IsValid;

    protected override void Awake()
    {
        base.Awake();
        _pointerData = new PointerEventData(eventSystem) { pointerId = VIRTUAL_POINTER_ID };
    }

    public void SetInitialPosition(Vector2 position)
    {
        _pointerData.position = position;
        _pointerData.delta = Vector2.zero;
    }

    public override void Process()
    {
        if (!IsValid) return;
        
        // Debug.Log(_pointerData.position);
        _pointerData.delta = CursorPosition - _pointerData.position;
        _pointerData.position = CursorPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(_pointerData, results);
        _pointerData.pointerCurrentRaycast = results.Count > 0 ? results[0] : new RaycastResult();
        HandlePointerExitAndEnter(_pointerData, _pointerData.pointerCurrentRaycast.gameObject);

        if (IsPressed) processPress();
        if (IsReleased) processRelease();
        return;

        void processPress()
        {
            IsPressed = false;
            
            GameObject target = _pointerData.pointerCurrentRaycast.gameObject;
            _pointerData.pointerPressRaycast = _pointerData.pointerCurrentRaycast;

            GameObject pressed = ExecuteEvents.ExecuteHierarchy(target, _pointerData, ExecuteEvents.pointerDownHandler);
            if (pressed == null)
            {
                pressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(target);
            }

            _pointerData.pointerPress = pressed;
            _pointerData.rawPointerPress = target;
            _pointerData.eligibleForClick = true;
            _pointerData.useDragThreshold = true;
        }

        void processRelease()
        {
            IsReleased = false;
            
            GameObject target = _pointerData.pointerCurrentRaycast.gameObject;
            GameObject pressed = _pointerData.pointerPress;

            ExecuteEvents.Execute(pressed, _pointerData, ExecuteEvents.pointerUpHandler);

            GameObject clickTarget = ExecuteEvents.GetEventHandler<IPointerClickHandler>(target);
            if (pressed == clickTarget && _pointerData.eligibleForClick)
            {
                ExecuteEvents.Execute(pressed, _pointerData, ExecuteEvents.pointerClickHandler);
            }
            //TODO
            // eventSystem.SetSelectedGameObject(null); //change somehow to only deselect when leaving the object

            _pointerData.pointerPress = null;
            _pointerData.rawPointerPress = null;
            _pointerData.eligibleForClick = false;
        }
    }
    
    public override bool IsPointerOverGameObject(int pointerId) => _pointerData.pointerEnter != null;
}
