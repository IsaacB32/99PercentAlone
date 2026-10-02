using UnityEngine;

namespace ITween.Animator.Editor
{
    using UnityEditor;
    using NaughtyAttributes.Editor;

    [CustomEditor(typeof(TweenAnimator), true)]
    public class TweenAnimatorEditor : NaughtyInspector
    {
        private GUILayoutOption _layoutOption;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _layoutOption = GUILayout.MaxWidth(250f);
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            TweenAnimator tweenTarget = (TweenAnimator)target;
        
            GUI.enabled = Application.isPlaying;
            
            EditorGUILayout.BeginHorizontal();
        
            if (GUILayout.Button("Show", _layoutOption))
            {
                tweenTarget.SetVisible(false, false);
                tweenTarget.SetVisible(true, onComplete: () => Debug.Log("show finished"));
            }
            
            if (GUILayout.Button("Hide", _layoutOption))
            {
                tweenTarget.SetVisible(true, false);
                tweenTarget.SetVisible(false, onComplete: () => Debug.Log("hide finished"));
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Set Visible", _layoutOption))
            {
                tweenTarget.SetVisible(true, false);
            }
            
            if (GUILayout.Button("Set Hidden", _layoutOption))
            {
                tweenTarget.SetVisible(false, false);
            }
            
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
        }
    }
}
