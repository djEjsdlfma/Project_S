using MoonLib.ScriptFinder_Pro.RunTime.Attribute;
using MoonLib.ScriptFinder_Pro.RunTime.Serializable;
using UnityEngine;

namespace MoonLib.ScriptFinder_Pro.RunTime.Finder.OneFinder
{
    public class ScriptOneFinderBase : ScriptFinderBase
    {
        
#if UNITY_EDITOR
        [field:InfoBox("Types must be not null", InfoType.Warning, nameof(_isWarning))]
#endif
        [field:Header("MonoBehaviour or Interface type to find in the scene")]
        [field: SerializeField]public SerializableType KeyType { get; protected set; }
        
        public override void Initialize() { }
        
        private void OnValidate()
        {
            _isWarning = KeyType.Type == null;
            if(KeyType.Type == null) return;
            if (KeyType.Type.IsAbstract || KeyType.Type.IsInterface)
            {
                IsFindChild = true;
            }
        }
    }
}