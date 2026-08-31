using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EditorOnlyAttribute))]
public class EditorOnlyAttributeDrawer : PropertyDrawerBase
{
    protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
    {
        bool isDisabled = Application.isPlaying;
        using (new EditorGUI.DisabledGroupScope(isDisabled))
        {
            EditorGUI.PropertyField(rect, property, label);
        }
    }
}
