using UnityEngine;

namespace ITween.Editor
{
    using UnityEditor;
    
    [CustomEditor(typeof(ITweenManager.ITweenRunner))]
    public class TweenRunnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ITweenManager.ITweenRunner runner = (ITweenManager.ITweenRunner)target;

            int amount = serializedObject.FindProperty("_activeAmount").intValue;
            EditorGUILayout.LabelField($"Active Tween Count: {amount}");
        }
    }
}
