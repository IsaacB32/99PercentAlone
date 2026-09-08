using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class IndentAttribute : PropertyAttribute
{
   public int IndentLevel { get; }

   public IndentAttribute(int indentLevel = 1)
   {
      IndentLevel = indentLevel;
   }
}
