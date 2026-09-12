using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuInputController : InputController
{
    private static MenuInputController _controller => InputEngine.GetMenuController();
    
    [field: SerializeField, ReadOnly] public Menu ActiveRootMenu { get; private set; }
    public InputMapType PreviousInputType { get; private set; }

    private Action OnRootClose;
    public Menu ActiveMenu => _menuStack.TryPeek(out Menu menu) ? menu : null;
    private Stack<Menu> _menuStack = new Stack<Menu>();
    
    [Space]
    [SerializeField] private MenuUIInputModule _menuInputModule;
    [SerializeField] private float _virtualCursorSpeed = 10f;

    private MenuVirtualCursor _activeCursor;
    
    //===== Input Properties =====
    public Vector2 MouseDelta { get; private set; }

    //===== State Machine =====
    
    public override void OnEnter(InputMapType oldType)
    {
        OpenMenuAndSelect(ActiveRootMenu);
        _menuStack.Push(ActiveRootMenu);
        
        _menuInputModule.IsValid = true;
    }

    public override void OnExit(InputMapType newType)
    {
        OnRootClose = null;
        ActiveRootMenu = null;
        _activeCursor = null;
        _menuStack.Clear();
        
        _menuInputModule.IsValid = false;
    }
    
    //===== Input Action Callbacks =====

    public void ExitMenu(InputAction.CallbackContext context)
    {
        if (InputEngine.InputLock) return;
        
        if (context.performed)
        {
            CloseActiveMenu();
        }
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (InputEngine.InputLock) return;
        
        // Skip when the cursor is unlocked so menus don't yank the camera around
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            MouseDelta = Vector2.zero;
            return;
        }
      
        Vector2 mouse = context.ReadValue<Vector2>();
        float mouseX = mouse.x * (1 / _virtualCursorSpeed);
        float mouseY = mouse.y * (1 / _virtualCursorSpeed);

        Vector2 activeMouse = new Vector2(mouseX, mouseY);
        MouseDelta = activeMouse;
        
        _activeCursor.MoveCursor(MouseDelta, ActiveMenu.MenuBounds);
    }

    public void Select(InputAction.CallbackContext context)
    {
        if (InputEngine.InputLock) return;
        
        _activeCursor.SelectCursor(context);
    }
    
    //===== Control =====
    
    private void OpenMenuAndSelect(Menu menu)
    {
        _activeCursor = menu.Cursor;
        _menuInputModule.SetInitialPosition(menu.Cursor.InitialPosition());
        menu.OpenMenu();
    }
    
    /// <summary>
    /// Open a new Menu object and switch controls to Menu
    /// </summary>
    public static void OpenMenuAsRoot(Menu menu, Action onClose)
    {
        MenuInputController controller = InputEngine.GetMenuController();
        Menu activeRoot = controller.ActiveRootMenu;
        
        if (menu == activeRoot) return;
        if (menu.State != Menu.MenuState.Closed) return;
        if (activeRoot != null) throw new Exception($"Menu: {activeRoot} is currently open, use {nameof(OpenMenuAsChild)} to open a sub menu");
        if (controller._menuStack.Count != 0) throw new Exception($"_menuStack must be zero for root menu : ({controller._menuStack.Count})");
        
        controller.OnRootClose = onClose;
        controller.ActiveRootMenu = menu;
        controller.PreviousInputType = InputEngine.SwitchActionMap(InputMapType.Menu);
    }

    /// <summary>
    /// Open a new child Menu object with a parent 
    /// </summary>
    public static void OpenMenuAsChild(Menu child)
    {
        MenuInputController controller = _controller;
        Menu parent = controller.ActiveMenu;

        if (parent == null) throw new Exception($"No menu is open, use {nameof(OpenMenuAsRoot)} to open a new root menu");
        if (parent.State != Menu.MenuState.Open) return;

        parent.ParentMenu();
        controller.OpenMenuAndSelect(child);
        controller._menuStack.Push(child);
    }
    
    /// <summary>
    /// Close a menu and exit Menu Input as if it was exiting the root
    /// </summary>
    public static void CloseMenuAsRoot()
    {
        CloseMenu(_controller.ActiveRootMenu);
    }
    
    /// <summary>
    /// Close a menu and all its children 
    /// </summary>
    public static void CloseMenu(Menu menu)
    {
        int failsafeCount = 0;
        MenuInputController controller = _controller;

        if (menu.State == Menu.MenuState.Closed) return;
        if (!controller._menuStack.Contains(menu)) return;

        //close all Menus above menu in the stack 
        while (controller.ActiveMenu != menu)
        {
            bool wasClosed = CloseActiveMenu();
            if (!wasClosed) throw new Exception("Menu failed to close, invalid stack");
            
            failsafeCount++;
            if (failsafeCount > 100) throw new Exception("loop exceeded max loop amount");
        }
        
        //close menu
        CloseActiveMenu();
    }

    /// <summary>
    /// Close the active menu are fire its OnClose event
    /// </summary>
    /// <returns>true if menu was closed and removed from stack</returns>
    public static bool CloseActiveMenu()
    {
        MenuInputController controller = _controller;
        Menu activeMenu = controller.ActiveMenu;

        if (activeMenu == null) return false;
        if (activeMenu.State == Menu.MenuState.Closed) return false;
        
        activeMenu.CloseMenu();
        controller._menuStack.Pop();

        if (controller._menuStack.TryPeek(out Menu childMenu)) controller.OpenMenuAndSelect(childMenu);
        else
        {
            InputEngine.InputLock.RegisterLockHolder(controller);
            controller.OnRootClose.Invoke();
        }
        return true;
    }
    
    /// <summary>
    /// Return regular inputs when Menu animations are done
    /// </summary>
    public static void FinishCloseMenu()
    {
        MenuInputController controller = _controller;
        InputEngine.SwitchActionMap(controller.PreviousInputType);
        InputEngine.InputLock.UnregisterLockHolder(controller);
    }
}