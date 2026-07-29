using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GravityOrbiter))]
public class OrbitHandles : NaughtyInspector
{
    private void OnSceneGUI()
    {
        GravityOrbiter t = (GravityOrbiter)target;
        if (!t.RenderOrbit) return;

        EditorGUI.BeginChangeCheck();
        Quaternion rot = Handles.RotationHandle(t.Rotation, t.FindCenterOfMass());
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Rotated RotateAt Point");
            t.Rotation = rot;
            t.OnValidate();
        }
    }
}
