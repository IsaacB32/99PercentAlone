// Source - https://stackoverflow.com/a/77920674
// Posted by Ujjwal Raut, modified by community. See post 'Timeline' for change history
// Retrieved 2026-07-12, License - CC BY-SA 4.0

using System;
using UnityEngine;

namespace Isaac.Attributes
{
    /// <summary>
    /// Make the property ReadOnly
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute
    {
        public const string INVALID_ID = "-1Invalid";
        
        public readonly string FieldName;
        public readonly bool RequiredValue;
        public readonly bool HiddenFlag;

        public ReadOnlyAttribute()
        {
            FieldName = INVALID_ID;
        }
        
        public ReadOnlyAttribute(string fieldName, bool hidden = false, bool requiredValue = true)
        {
            FieldName = fieldName;
            RequiredValue = requiredValue;
            HiddenFlag = hidden;
        }
    }
}