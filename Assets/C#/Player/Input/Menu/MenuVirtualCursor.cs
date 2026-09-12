using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuVirtualCursor : MonoBehaviour
{
    public Vector2 InitialPosition() => _rectTransform.anchoredPosition;
    
    private RectTransform _rectTransform;
    private Func<Vector2> _screenPoint;
    private Vector2 _padding;
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        Camera eventCamera = Camera.main;

        RenderMode renderType = GetComponentInParent<Canvas>().renderMode;
        _screenPoint = renderType switch
        {
            RenderMode.WorldSpace => () => eventCamera.WorldToScreenPoint(_rectTransform.position),
            _ => () => RectTransformUtility.WorldToScreenPoint(null, _rectTransform.position)
        };

        _padding.x = _rectTransform.rect.width / 2f;
        _padding.y = _rectTransform.rect.height / 2f;
        
        transform.SetAsLastSibling();
    }

    //===== Control =====
    
    public void MoveCursor(Vector2 delta, Rect bounds)
    {
        Vector2 newPosition = _rectTransform.anchoredPosition + delta * .005f;
        newPosition.x = Mathf.Clamp(newPosition.x, bounds.xMin + _padding.x, bounds.xMax - _padding.x);
        newPosition.y = Mathf.Clamp(newPosition.y, bounds.yMin + _padding.x, bounds.yMax - _padding.y);
        _rectTransform.anchoredPosition = newPosition;
        
        MenuUIInputModule.CursorPosition = _screenPoint.Invoke();
    }
    
    public void SelectCursor(InputAction.CallbackContext context)
    {
        if (context.performed) MenuUIInputModule.IsPressed = true;
        else if (context.canceled) MenuUIInputModule.IsReleased = true;
    }
}
