using UnityEngine;

namespace ITween.Animator.Editor
{
    using UnityEditor;
    using NaughtyAttributes.Editor;
    
    /// <summary>
    /// Custom Editor for a TweenAnimator to draw playback buttons 
    /// </summary>
    [CustomEditor(typeof(UITweenAnimator), true)]
    public class UITweenAnimatorEditor : NaughtyInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            UITweenAnimator tweenAnimator = (UITweenAnimator)target;

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = Application.isPlaying;
            
            if (GUILayout.Button("Show"))
            {
                tweenAnimator.ActiveTween.SetVisible(true);
            }

            if (GUILayout.Button("Hide"))
            {
                tweenAnimator.ActiveTween.SetVisible(false);
            }

            if (GUILayout.Button("Reset"))
            {
                tweenAnimator.ActiveTween.SetVisible(false, false);
            }
            
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }
    }
}
