using UnityEditor;
using UnityEngine;

public static class EditorExtensions 
{
   public static void DrawDefaultScriptProperty(Object target)
   {
      using (new EditorGUI.DisabledGroupScope(true))
      {
         EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), target.GetType(), false);
      }
   }
}
