using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class IntentAttribute : PropertyAttribute
{
   public int IndentLevel { get; }

   public IntentAttribute(int indentLevel = 1)
   {
      IndentLevel = indentLevel;
   }
}
