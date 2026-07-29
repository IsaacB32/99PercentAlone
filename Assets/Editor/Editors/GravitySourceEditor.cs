using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GravitySource), true)]
public class GravitySourceEditor : NaughtyInspector
{
    private const string GRAVITY_WARNING = "Gravity Source is not connected to Gravity Field, will be ignored in physics calculations"; 
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GravitySource source = (GravitySource)target;
        if (PrefabUtility.IsPartOfPrefabAsset(source)) return;
        if (!source.transform.parent || !source.transform.parent.GetComponent<GravityFieldTrigger>())
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(GRAVITY_WARNING, MessageType.Warning);
            if (GUILayout.Button("Assign to Gravity Field"))
            {
                GravityFieldTrigger field = FindAnyObjectByType<GravityFieldTrigger>(FindObjectsInactive.Exclude);
                if (!field)
                {
                    GameObject newField = new GameObject("Gravity Field")
                    {
                        layer = Layers.GravityTrigger,
                        transform =
                        {
                            position = source.transform.position
                        }
                    };
                    field = newField.AddComponent<GravityFieldTrigger>();
                }
                
                source.transform.SetParent(field.transform);
            }
        }
    }
}
