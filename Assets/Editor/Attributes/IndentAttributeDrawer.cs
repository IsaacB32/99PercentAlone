using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IndentAttribute))]
public class IndentAttributeDrawer : PropertyDrawerBase
{
    protected override void OnGUI_Internal(Rect position, SerializedProperty property, GUIContent label)
    {
        IndentAttribute indent = attribute as IndentAttribute;
        
        EditorGUI.indentLevel += indent.IndentLevel;
        EditorGUI.PropertyField(position, property, label);
        EditorGUI.indentLevel -= indent.IndentLevel;
    }
}
