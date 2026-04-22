using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoonLib.ScriptFinder_Pro.RunTime.Finder
{
    public enum FinderType
    {
        None,
        Normal,
        All,
        List,
        ListAll
    }
    public abstract class ScriptFinderBase : ScriptableObject
    {
        protected MonoBehaviour _target;
        protected List<MonoBehaviour> _targets = new();
        protected Dictionary<Type, MonoBehaviour> _targetDict = new();
        protected Dictionary<Type, List<MonoBehaviour>> _targetsDict = new();
        
        [field: SerializeField]public bool IsFindChild { get; protected set; } = false;
        
        public FinderType FinderType { get; protected set; } = FinderType.None;
        
        protected Transform _targetTransform;
        protected List<Transform> _targetsTransform = new();
        protected Dictionary<Type, Transform> _targetTransformDict = new();
        protected Dictionary<Type, List<Transform>> _targetsTransformDict = new();
        
        [SerializeField, HideInInspector]
        protected bool _isWarning = false;
        
        public abstract void Initialize();
    }
}
