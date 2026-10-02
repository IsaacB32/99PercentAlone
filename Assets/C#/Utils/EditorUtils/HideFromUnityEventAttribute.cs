using System;

/// <summary>
/// doesn't do anything just a marker Attribute
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class HideFromUnityEventAttribute : Attribute
{
    //=!= Documentation Attribute =!=
    // for marking why some methods have a [SpecialName] attribute
    
    //to use HideFromUnityEvent:
    
    //[HideFromUnityEvent, SpecialName]
    //public void CalledFromCodeOnly() { ... }
    
    //can also mark the method as internal
}
