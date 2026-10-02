using System;
using ITween.Animator;
using UnityEngine;

[RequireComponent(typeof(TransformAnimator))]
public abstract class Tool : MonoBehaviour
{
    [SerializeField] private MenuInteractable _toolMenu;
    private TransformAnimator _transformAnimator;

    private void Awake()
    {
        _transformAnimator = GetComponent<TransformAnimator>();
    }
    
    #region Subscribe

    private void OnEnable()
    {
        _toolMenu.OnSelectEnd += UseTool;
    }

    private void OnDisable()
    {
        _toolMenu.OnSelectEnd -= UseTool;
    }

    #endregion

    public void OperateTool(bool isActive)
    {
        if (isActive) OpenTool();
        else CloseTool();
    }
    
    private void OpenTool()
    {
        InputEngine.InteractionLock.RegisterLockHolder(this);
        _transformAnimator.SetVisible(true);
    }

    private void CloseTool()
    {
        InputEngine.InteractionLock.UnregisterLockHolder(this);
        _transformAnimator.SetVisible(false);
    }
    
    protected abstract void UseTool();
}
