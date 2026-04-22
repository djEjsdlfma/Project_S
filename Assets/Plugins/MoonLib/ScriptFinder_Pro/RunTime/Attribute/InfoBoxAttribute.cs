using System;
using UnityEngine;

namespace MoonLib.ScriptFinder_Pro.RunTime.Attribute
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class InfoBoxAttribute : PropertyAttribute
    {
        public readonly string Message;
        public readonly InfoType Type;
        public readonly string VisibleIf;

        public InfoBoxAttribute(string message, InfoType type = InfoType.Info, string visibleIf = null)
        {
            Message = message;
            Type = type;
            VisibleIf = visibleIf;
        }
    }

    public enum InfoType
    {
        None,
        Info,
        Warning,
        Error,
    }
}