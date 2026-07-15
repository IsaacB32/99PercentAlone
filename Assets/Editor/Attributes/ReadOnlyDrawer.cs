// Source - https://stackoverflow.com/a/77920674
// Posted by Ujjwal Raut, modified by community. See post 'Timeline' for change history
// Retrieved 2026-07-12, License - CC BY-SA 4.0

using UnityEngine;
using UnityEditor;

namespace Isaac.Attributes.Editor
{
    /// <summary>
    /// Draw ReadOnly properties according to their parameters
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ReadOnlyAttribute target = (ReadOnlyAttribute)attribute;
            if (target.HiddenFlag && !IsEnabled(target, property)) return 0;
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReadOnlyAttribute target = (ReadOnlyAttribute)attribute;
            bool enableProperty = IsEnabled(target, property);

            if (target.HiddenFlag && !enableProperty) return;
            
            GUI.enabled = enableProperty;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }

        private static bool IsEnabled(ReadOnlyAttribute target, SerializedProperty property)
        {
            bool enableProperty = false;
            if (!target.FieldName.Equals(ReadOnlyAttribute.INVALID_ID))
            {
                SerializedProperty controller = property.serializedObject.FindProperty(target.FieldName);
                if (controller is null)
                {
                    string warning = $"Property {target.FieldName} could not be found";
                    EditorGUILayout.HelpBox(warning, MessageType.Warning);
                }
                else if (controller.propertyType is not SerializedPropertyType.Boolean)
                {
                    string warning = $"Property type must be {typeof(bool)}";
                    EditorGUILayout.HelpBox(warning, MessageType.Warning);
                }
                else
                {
                    enableProperty = controller.boolValue == target.RequiredValue;
                }
            }

            return enableProperty;
        }
    }
}

