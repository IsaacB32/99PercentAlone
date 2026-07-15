using System;
using JetBrains.Annotations;

namespace Isaac.Attributes
{
    /// <summary>
    /// Draw a method as a button in the Unity inspector
    /// </summary>
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public class FunctionButtonAttribute : Attribute
    {
        public string Label { get; }

        public FunctionButtonAttribute(string label = null)
        {
            Label = label;
        }
    }
}
