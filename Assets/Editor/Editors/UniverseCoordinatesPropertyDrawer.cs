using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UniverseCoordinates))]
public class UniverseCoordinatesPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty x = property.FindPropertyRelative("x");
        SerializedProperty y = property.FindPropertyRelative("y");
        SerializedProperty z = property.FindPropertyRelative("z");

        Vector3Int value = new Vector3Int(x.intValue, y.intValue, z.intValue);

        EditorGUI.BeginChangeCheck();

        value = EditorGUI.Vector3IntField(position, label, value);

        if (EditorGUI.EndChangeCheck())
        {
            x.intValue = value.x;
            y.intValue = value.y;
            z.intValue = value.z;
        }
    }
}