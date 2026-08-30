using NaughtyAttributes.Editor;
using UnityEditor;

[CustomEditor(typeof(CameraControlInteractable), true)]
public class CameraControlInteractableEditor : NaughtyInspector
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorExtensions.DrawDefaultScriptProperty(target);
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_cameraTargetPoint"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_playerTargetPoint"));
        
        SerializedProperty iterator = serializedObject.GetIterator();
        if (iterator.NextVisible(true))
        {
            do
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (iterator.propertyPath.Equals("m_Script")) continue;
                if (iterator.propertyPath.Equals("_cameraTargetPoint")) continue;
                if (iterator.propertyPath.Equals("_playerTargetPoint")) continue;
                NaughtyEditorGUI.PropertyField_Layout(iterator, true);
            } 
            while (iterator.NextVisible(false));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
