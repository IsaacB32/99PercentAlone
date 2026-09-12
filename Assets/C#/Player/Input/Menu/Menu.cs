using System.Linq;
using System.Runtime.CompilerServices;
using NaughtyAttributes;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public static readonly Rect ScreenBounds = new Rect(-Screen.width/2f, -Screen.height/2f, Screen.width, Screen.height);
    
    [SerializeField] private string _menuName;
    [field: SerializeField, Required("Cursor Required")] public MenuVirtualCursor Cursor { get; private set; }

    public MenuState State { get; private set; }
    public Rect MenuBounds { get; private set; }

    private void Awake()
    {
        RectTransform menuTransform = GetComponent<RectTransform>();
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        MenuBounds = parentCanvas.renderMode == RenderMode.WorldSpace ? menuTransform.rect : ScreenBounds;
    }

    //===== Control ======
    
    public void OpenAsChild()
    {
        MenuInputController.OpenMenuAsChild(this);
    }

    public static void CloseAsRoot()
    {
        MenuInputController.CloseMenuAsRoot();
    }
    
    //===== Internal Controls =====
    
    [HideFromUnityEvent, SpecialName]
    public void OpenMenu()
    {
        State = MenuState.Open;
        OnPresent();
    }

    [HideFromUnityEvent, SpecialName]
    public void ParentMenu()
    {
        State = MenuState.Parented;
        OnDismiss();
    }

    [HideFromUnityEvent, SpecialName]
    public void CloseMenu()
    {
        State = MenuState.Closed;
        OnDismiss();
    }
    
    //===== Virtual =====
    
    /// <summary>
    /// For displaying animations or other callbacks when the menu is presented 
    /// </summary>
    protected virtual void OnPresent()
    {
        //override point
    }

    /// <summary>
    /// For displaying animations or other callbacks when the menu is closed
    /// </summary>
    protected virtual void OnDismiss()
    {
        //override point
    }
    
    //===== Housekeeping =====
    
    public override string ToString() => _menuName;

    public enum MenuState
    {
        Closed,     //not open at all
        Open,       //visible on the active UI
        Parented    //not visible but considered open
    }
}
