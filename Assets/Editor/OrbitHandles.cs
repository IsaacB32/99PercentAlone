using System;
using Isaac.Attributes.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GravityOrbiter))]
public class OrbitHandles : MonoBehaviourFunctionButtonEditor
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
