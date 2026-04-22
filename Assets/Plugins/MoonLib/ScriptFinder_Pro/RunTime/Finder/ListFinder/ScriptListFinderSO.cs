using System;
using System.Collections.Generic;
using System.Linq;
using MoonLib.ScriptFinder_Pro.RunTime.DevLogs;
using UnityEngine;

namespace MoonLib.ScriptFinder_Pro.RunTime.Finder.ListFinder
{
    [CreateAssetMenu(fileName = "ScriptListFinder",menuName = "ScriptFinder/List", order = 2)]
    public class ScriptListFinderSO : ScriptManyFinderBase
    {
        public override void Initialize()
        {
            FinderType = FinderType.List;
        }
        
        public void SetTarget(Dictionary<Type, MonoBehaviour> targetList)
        {
            _targetDict = targetList;
            _targetTransformDict = targetList
                .ToDictionary(pair => pair.Key, pair => pair.Value.transform);
            _targetsTransform = _targetTransformDict.Values.Select(t => t.transform).ToList();
        }
        
        public void AddTarget(MonoBehaviour target ,bool overwrite = false)
        {
            Type targetType = target.GetType();

            if (_targetDict.ContainsKey(targetType))
            {
                if (!overwrite)
                {
                    DevLog.LogError($"ScriptFinderSO: Target of type {targetType.Name} already exists. Set overwrite to true to replace it.");
                    return;
                }

                if (_targetTransformDict.TryGetValue(targetType, out var oldTransform))
                {
                    if (oldTransform != null && _targetsTransform.Contains(oldTransform))
                    {
                        _targetsTransform.Remove(oldTransform);
                    }
                }
            }

            _targetDict[targetType] = target;
            _targetTransformDict[targetType] = target.transform;

            if (!_targetsTransform.Contains(target.transform))
                _targetsTransform.Add(target.transform);
        }
        
        public T GetTarget<T>(bool isNullLog = true) where T : MonoBehaviour
        {
            if (!_targetDict.TryGetValue(typeof(T), out MonoBehaviour component))
            {
                if (isNullLog)
                    DevLog.LogError($"ScriptFinderSO: No target found for type {typeof(T).Name}.");
                return null;
            }

            if (component is T t) return t;

            DevLog.LogError($"ScriptFinderSO: Target is not of type {typeof(T).Name}. " +
                            $"Current type: {component?.GetType().Name ?? "null"}");
            return null;
        }

        
        public T GetInterface<T>(bool isNullLog = true) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                DevLog.LogError($"[ScriptFinderSO] Type {typeof(T).Name} is not an interface.\n" +
                                "This method requires an interface type. ");
                return null;
            }
            
            if (!_targetDict.TryGetValue(typeof(T), out MonoBehaviour component))
            {
                if (isNullLog)
                    DevLog.LogError($"ScriptFinderSO: No target found for interface {typeof(T).Name}.");
                return null;
            }

            if (component is T i) return i;

            DevLog.LogError($"ScriptFinderSO: Target does not implement interface {typeof(T).Name}. " +
                            $"Current type: {component?.GetType().Name ?? "null"}");
            return null;
        }
        
        private void AddTargetDict(Dictionary<Type, MonoBehaviour> targetsDict, bool overwrite = false)
        {
            if (targetsDict == null) return;

            foreach (var kv in targetsDict)
            {
                var target = kv.Value;
                if (target == null) continue;
                AddTarget(target, overwrite);
            }
        }

        public void AddTargetDict<T>(Dictionary<Type, T> targetsDict, bool overwrite = false) where T : MonoBehaviour
        {
            if (targetsDict == null) return;

            var converted = targetsDict.ToDictionary(
                kv => kv.Key,
                kv => kv.Value as MonoBehaviour
            );

            AddTargetDict(converted, overwrite);
        }

        
        public Transform GetTransform<T>() => _targetTransformDict[typeof(T)];
    }
}