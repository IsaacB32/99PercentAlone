using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IntentAttribute))]
public class IndentAttributeDrawer : PropertyDrawerBase
{
    protected override void OnGUI_Internal(Rect position, SerializedProperty property, GUIContent label)
    {
        IntentAttribute indent = attribute as IntentAttribute;
        
        EditorGUI.indentLevel += indent.IndentLevel;
        EditorGUI.PropertyField(position, property, label);
        EditorGUI.indentLevel -= indent.IndentLevel;
    }
}
