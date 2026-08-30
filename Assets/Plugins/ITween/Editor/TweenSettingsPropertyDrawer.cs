using UnityEditor;
using UnityEngine;

//=!= Custom Drawer for TweenSettings, shelved until I want to work on it again =!=

namespace ITween.Editor
{
    // [CustomPropertyDrawer(typeof(TweenSettings))]
    // public class TweenSettingsPropertyDrawer : PropertyDrawer
    // {
    //     private const string TOTAL_FOLD_KEY = "TotalFoldout";
    //     private const string ADVANCED_FOLD_KEY = "AdvancedFoldout";
    //     
    //     private static readonly Color BG_COLOR = new Color(0.255f, 0.255f, 0.255f);
    //     
    //     private static bool GetFoldoutState(string subKey, SerializedProperty property)
    //     {
    //         string key = $"TweenSettingsDrawer_{subKey}_{property.serializedObject.targetObject.GetEntityId()}_{property.propertyPath}";
    //         return SessionState.GetBool(key, false);
    //     }
    //
    //     private static void SetFoldoutState(string subKey, SerializedProperty property, bool value)
    //     {
    //         string key = $"TweenSettingsDrawer_{subKey}_{property.serializedObject.targetObject.GetEntityId()}_{property.propertyPath}";
    //         SessionState.SetBool(key, value);
    //     }
    //
    //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //     {
    //         TweenProperties properties = new TweenProperties(property);
    //         bool isTotalFolded = GetFoldoutState(TOTAL_FOLD_KEY, property);
    //         bool isAdvancedFolded = GetFoldoutState(ADVANCED_FOLD_KEY, property);
    //         
    //         EditorGUI.BeginProperty(position, label, property);
    //
    //
    //         Rect contentRect = new Rect(
    //             position.x + 8,
    //             position.y + 4,
    //             position.width - 16,
    //             position.height - 8
    //         );
    //         Rect itemRect = new Rect(contentRect.x, contentRect.y, contentRect.width, EditorGUIUtility.singleLineHeight);
    //
    //         isTotalFolded = EditorGUI.BeginFoldoutHeaderGroup(itemRect, isTotalFolded, label);
    //         SetFoldoutState(TOTAL_FOLD_KEY, property, isTotalFolded);
    //         
    //         if (isTotalFolded)
    //         {
    //             Rect bgRect = new Rect(
    //                 position.x,
    //                 position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
    //                 position.width,
    //                 position.height
    //             );
    //             EditorGUI.DrawRect(bgRect, BG_COLOR);
    //             
    //             EditorGUI.indentLevel++;
    //
    //             EditorGUI.PropertyField(autoIncreaseRect(), properties.duration);
    //             EditorGUI.PropertyField(autoIncreaseRect(), properties.easeType);
    //             EditorGUI.PropertyField(autoIncreaseRect(properties.overshoot), properties.overshoot);
    //             EditorGUI.PropertyField(autoIncreaseRect(properties.customCurve), properties.customCurve);
    //
    //             isAdvancedFolded = EditorGUI.Foldout(autoIncreaseRect(), isAdvancedFolded, "Advanced", true);
    //             SetFoldoutState(ADVANCED_FOLD_KEY, property, isAdvancedFolded);
    //         
    //             if (isAdvancedFolded)
    //             {
    //                 EditorGUI.indentLevel++;
    //                 EditorGUI.PropertyField(autoIncreaseRect(), properties.delayTime);
    //                 EditorGUI.PropertyField(autoIncreaseRect(), properties.loopingType);
    //                 EditorGUI.PropertyField(autoIncreaseRect(properties.loopCount), properties.loopCount);
    //                 EditorGUI.PropertyField(autoIncreaseRect(properties.hangTime), properties.hangTime);
    //                 EditorGUI.indentLevel--;
    //             }
    //
    //             EditorGUI.PropertyField(autoIncreaseRect(), properties.flags, true);
    //
    //             EditorGUI.indentLevel--;
    //         }
    //         
    //         EditorGUI.EndFoldoutHeaderGroup();
    //         EditorGUI.EndProperty();
    //         return;
    //
    //         Rect autoIncreaseRect(SerializedProperty p = null)
    //         {
    //             if (p != null) itemRect.y += EditorGUI.GetPropertyHeight(p);
    //             else itemRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    //             
    //             return new Rect(itemRect.x, itemRect.y, itemRect.width, EditorGUIUtility.singleLineHeight);
    //         }
    //     }
    //
    //     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //     {
    //         TweenProperties properties = new TweenProperties(property);
    //         bool isTotalFolded = GetFoldoutState(TOTAL_FOLD_KEY, property);
    //         bool isAdvancedFolded = GetFoldoutState(ADVANCED_FOLD_KEY, property);
    //         float height = 0f;
    //
    //         if (!isTotalFolded)
    //         {
    //             height += EditorGUIUtility.singleLineHeight + 8f;
    //             return height;
    //         }
    //
    //         height += EditorGUIUtility.singleLineHeight * 2f;
    //         
    //         height += EditorGUI.GetPropertyHeight(properties.duration) + EditorGUIUtility.standardVerticalSpacing;
    //         height += EditorGUI.GetPropertyHeight(properties.easeType) + EditorGUIUtility.standardVerticalSpacing;
    //         height += EditorGUI.GetPropertyHeight(properties.overshoot) + EditorGUIUtility.standardVerticalSpacing;
    //         height += EditorGUI.GetPropertyHeight(properties.customCurve) + EditorGUIUtility.standardVerticalSpacing;
    //
    //         if (isAdvancedFolded)
    //         {
    //             height += EditorGUI.GetPropertyHeight(properties.delayTime) + EditorGUIUtility.standardVerticalSpacing;
    //             height += EditorGUI.GetPropertyHeight(properties.loopingType) + EditorGUIUtility.standardVerticalSpacing;
    //             height += EditorGUI.GetPropertyHeight(properties.loopCount) + EditorGUIUtility.standardVerticalSpacing;
    //             height += EditorGUI.GetPropertyHeight(properties.hangTime) + EditorGUIUtility.standardVerticalSpacing;
    //         }
    //
    //         height += EditorGUI.GetPropertyHeight(properties.flags) + EditorGUIUtility.standardVerticalSpacing;
    //
    //         height += 8f; //padding bottom
    //         return height;
    //     }
    //
    //     struct TweenProperties
    //     {
    //         public SerializedProperty duration;
    //         public SerializedProperty easeType;
    //         public SerializedProperty overshoot;
    //         public SerializedProperty customCurve;
    //         
    //         public SerializedProperty delayTime;
    //         public SerializedProperty loopingType;
    //         public SerializedProperty loopCount;
    //         public SerializedProperty hangTime;
    //         
    //         public SerializedProperty flags;
    //
    //         public TweenProperties(SerializedProperty property)
    //         {
    //             duration = property.FindPropertyRelative("_duration");
    //             easeType = property.FindPropertyRelative("_easeType");
    //             overshoot = property.FindPropertyRelative("_overshoot");
    //             customCurve = property.FindPropertyRelative("_customCurve");
    //
    //             delayTime = property.FindPropertyRelative("_delayTime");
    //             loopingType = property.FindPropertyRelative("_loopingType");
    //             loopCount = property.FindPropertyRelative("_loopCount");
    //             hangTime = property.FindPropertyRelative("_hangTime");
    //
    //             flags = property.FindPropertyRelative("_flags");
    //         }
    //     }
    // }
}
