using System.Collections.Generic;
using System.Linq;
using MoonLib.ScriptFinder_Pro.RunTime.Attribute;
using UnityEngine;

namespace MoonLib.ScriptFinder_Pro.RunTime.Finder.ListFinder
{
    public class ScriptManyFinderBase : ScriptFinderBase
    {
#if UNITY_EDITOR
        [field:InfoBox("Types must be unique and not null", InfoType.Warning, nameof(_isWarning))]
#endif
        [field:Header("MonoBehaviour or Interface type to find in the scene")]
        [field:SerializeField]
        public List<ChildSerializableType> KeyType { get; protected set; }
        
        protected bool _isFindedChildBool = false;
        
        public override void Initialize() { }
        
        public List<Transform> GetAllTransform() => _targetsTransform;
        
        private void OnValidate()
        {
            _isWarning = !CheckTypeUniqueAndNotNull();
            
            if (KeyType == null) return;
            if (IsFindChild)
            {
                if(_isFindedChildBool) return;
                _isFindedChildBool = true;
                KeyType.ForEach(k => k.IsFindChild = true);
                return;
            }
            
            if (_isFindedChildBool)
            {
                _isFindedChildBool = false;
                KeyType.ForEach(k => k.IsFindChild = false);
            }
            
            foreach (var type in KeyType)
            {
                CheckTypeToIsChildSet(type);
            }
        }
        
        protected virtual bool CheckTypeUniqueAndNotNull()
        {
            var notUniqueOrNullTypes = KeyType
                .GroupBy(k => k.SType.Type)
                .Where(g => g.Key == null || g.Count() > 1);
            return !notUniqueOrNullTypes.Any();
        }

        protected virtual void CheckTypeToIsChildSet(ChildSerializableType type)
        {
            if(type.SType.Type == null) return;
            if(type.SType.Type.IsAbstract || type.SType.Type.IsInterface)
                type.IsFindChild = true;
        }
    }
}