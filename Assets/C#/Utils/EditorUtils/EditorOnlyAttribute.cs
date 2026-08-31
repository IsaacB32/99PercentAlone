using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class EditorOnlyAttribute : PropertyAttribute
{
    public EditorOnlyAttribute() { }
}
