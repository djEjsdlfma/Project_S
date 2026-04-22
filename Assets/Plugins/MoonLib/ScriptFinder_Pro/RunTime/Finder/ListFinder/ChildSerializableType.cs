using System;
using MoonLib.ScriptFinder_Pro.RunTime.Serializable;

namespace MoonLib.ScriptFinder_Pro.RunTime.Finder.ListFinder
{
    [Serializable]
    public class ChildSerializableType
    {
        public SerializableType SType;
        public bool IsFindChild = false;
    }
}