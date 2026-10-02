using System;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(CameraAnimator))]
public class MenuInteractable : MonoBehaviour
    , IInteractable
{
    [SerializeField, Required] private Menu _menu;
    [SerializeField] private bool _animate = true;
    private CameraAnimator _cameraAnimator;

    public event Action OnSelectBegin;
    public event Action OnSelectEnd;

    private void Awake()
    {
        _cameraAnimator = GetComponent<CameraAnimator>();
    }
    
    public void OnSelect()
    {
        OnSelectBegin?.Invoke();
        _cameraAnimator.AnimateToTarget(_animate, () =>
        {
            MenuInputController.OpenMenuAsRoot(_menu, ReturnToOrigin);
            OnSelectEnd?.Invoke();
        });
    }

    private void ReturnToOrigin()
    {
        _cameraAnimator.AnimateToOrigin(_animate, MenuInputController.FinishCloseMenu);
    }
}
