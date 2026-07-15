using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Isaac.Attributes.Editor
{
    internal static class FunctionButtonDrawer
    {
        public static void Draw(UnityEditor.Editor editor)
        {
            MethodInfo[] methods = editor.target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (MethodInfo method in methods)
            {
                FunctionButtonAttribute functionButtonAttr = method.GetCustomAttribute<FunctionButtonAttribute>();
                if (functionButtonAttr == null) continue;

                // throw warning if a parameter is found on the method  
                if (method.GetParameters().Length > 0)
                {
                    EditorGUILayout.HelpBox($"'{method.Name}' has parameters and cannot be bound to a button.", MessageType.Warning);
                    continue;
                }
                
                string label = string.IsNullOrEmpty(functionButtonAttr.Label) ? ObjectNames.NicifyVariableName(method.Name) : functionButtonAttr.Label;
                if (GUILayout.Button(label))
                {
                    foreach (Object t in editor.targets)
                    {
                        method.Invoke(t, null);
                    }
                }
            }
        }
    }

    [CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects]
    public class MonoBehaviourFunctionButtonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            FunctionButtonDrawer.Draw(this);
        }
    }

    [CustomEditor(typeof(ScriptableObject), true)]
    [CanEditMultipleObjects]
    public class ScriptableObjectFunctionButtonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            FunctionButtonDrawer.Draw(this);
        }
    }
}