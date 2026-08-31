using UnityEngine;

namespace ITween.Animator.Editor
{
    using UnityEditor;

    /// <summary>
    /// Custom Editor for a TweenAnimator to draw playback buttons 
    /// </summary>
    [CustomEditor(typeof(TweenAnimator), true)]
    public class TweenAnimatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TweenAnimator tweenAnimator = (TweenAnimator)target;

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = Application.isPlaying;
            
            if (GUILayout.Button("Start"))
            {
                tweenAnimator.ActiveTween.Start();
            }

            if (GUILayout.Button("Stop"))
            {
                tweenAnimator.ActiveTween.Stop();
            }

            if (GUILayout.Button("Restart"))
            {
                Tween.IT_ForceReturn(tweenAnimator.ActiveTween);
                tweenAnimator.ActiveTween.Restart();
            }
            
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }
    }
}
