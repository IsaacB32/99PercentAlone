using Isaac.Extensions;
using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraAnimator))]
public class CameraAnimatorEditor : NaughtyInspector
{
    private Transform _mainCameraTransform;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        _mainCameraTransform = Camera.main.transform;

        EditorApplication.playModeStateChanged += OnStateChange;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (CameraAnimator.IsPreview)
        {
            CameraAnimator.IsPreview = false;
            StopPreview();
        }
        
        EditorApplication.playModeStateChanged -= OnStateChange;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CameraAnimator animator = (CameraAnimator)target;
        Transform targetPoint = animator.Target;
        GUI.enabled = !EditorApplication.isPlaying && targetPoint;

        string label = !CameraAnimator.IsPreview ? "Preview Camera Target" : "Stop Preview";
        if (GUILayout.Button(label))
        {
            CameraAnimator.IsPreview = !CameraAnimator.IsPreview;
            if (CameraAnimator.IsPreview) StartPreview(targetPoint);
            else StopPreview();
        }
        
        GUI.enabled = true;
    }

    private void StartPreview(Transform targetPoint)
    {
        CameraAnimator.Cache = _mainCameraTransform.ToSnapshot();
                
        _mainCameraTransform.position = targetPoint.position;
        _mainCameraTransform.rotation = targetPoint.rotation;
    }

    private void StopPreview()
    {
        if (!CameraAnimator.Cache.IsValid) return;
        
        _mainCameraTransform.position = CameraAnimator.Cache.position;
        _mainCameraTransform.rotation = CameraAnimator.Cache.rotation;
    }
    
    private void OnStateChange(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            if (CameraAnimator.IsPreview)
            {
                CameraAnimator.IsPreview = false;
                StopPreview();
            }
        }
    }
}
