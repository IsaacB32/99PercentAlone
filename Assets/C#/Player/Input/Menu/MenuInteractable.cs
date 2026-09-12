using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(CameraAnimator))]
public class MenuInteractable : MonoBehaviour
    , IInteractable
{
    [SerializeField, Required] private Menu _menu;
    [SerializeField] private bool _animate = true;
    private CameraAnimator _cameraAnimator;

    private void Awake()
    {
        _cameraAnimator = GetComponent<CameraAnimator>();
    }
    
    public void OnSelect()
    {
        _cameraAnimator.AnimateToTarget(_animate, () =>
        {
            MenuInputController.OpenMenuAsRoot(_menu, ReturnToOrigin);
        });
    }

    private void ReturnToOrigin()
    {
        _cameraAnimator.AnimateToOrigin(_animate, MenuInputController.FinishCloseMenu);
    }
}
