using UnityEngine;

namespace ITween.Editor
{
    using UnityEditor;
    
    [CustomEditor(typeof(ITManager.ITweenRunner))]
    public class TweenRunnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ITManager.ITweenRunner runner = (ITManager.ITweenRunner)target;

            int amount = serializedObject.FindProperty("_activeAmount").intValue;
            EditorGUILayout.LabelField($"Active Tween Count: {amount}");
        }
    }
}
