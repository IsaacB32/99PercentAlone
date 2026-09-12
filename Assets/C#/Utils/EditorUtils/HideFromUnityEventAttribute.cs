using System;

[AttributeUsage(AttributeTargets.Method)]
public class HideFromUnityEventAttribute : Attribute
{
    //=!= Documentation Attribute =!=
    // for marking why some methods have a [SpecialName] attribute
    
    //to use HideFromUnityEvent:
    
    //[HideFromUnityEvent, SpecialName]
    //public void CalledFromCodeOnly() { ... }
}
