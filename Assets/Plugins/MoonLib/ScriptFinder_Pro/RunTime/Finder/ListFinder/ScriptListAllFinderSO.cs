using System;
using System.Collections.Generic;
using System.Linq;
using MoonLib.ScriptFinder_Pro.RunTime.DevLogs;
using UnityEngine;

namespace MoonLib.ScriptFinder_Pro.RunTime.Finder.ListFinder
{
    [CreateAssetMenu(fileName = "ScriptListAllFinder",menuName = "ScriptFinder/ListAll", order = 0)]
    public class ScriptListAllFinderSO : ScriptManyFinderBase
    {
        public override void Initialize()
        {
            FinderType = FinderType.ListAll;
        }
        
        public void SetTarget(Dictionary<Type, List<MonoBehaviour>> targetsList)
        {
            _targetsDict = targetsList;
            _targetsTransformDict = targetsList
                .ToDictionary(pair => pair.Key, pair => pair.Value.ConvertAll(t => t.transform));
            _targetsTransform = targetsList.Values
                .SelectMany(list => list)
                .Where(mb => mb != null)    
                .Select(mb => mb.transform) 
                .ToList();
        }   
        
        public void AddTarget(MonoBehaviour target)
        {
            Type targetType = target.GetType();
            
            if (KeyType.All(t => t.GetType() != targetType))
            {
                DevLog.LogError($"ScriptFinderSO: Target is not of a valid type. Current type: {targetType.Name}");
                return;
            }

            if (_targetsDict.ContainsKey(targetType))
            {
                if (!_targetsDict[targetType].Contains(target))
                {
                    _targetsDict[targetType].Add(target);
                    _targetsTransformDict[targetType].Add(target.transform);
                }
                else
                {
                    DevLog.LogError($"ScriptFinderSO: Target of type {targetType.Name} already exists in the list.");
                }
            }
            else
            {
                _targetsDict[targetType] = new List<MonoBehaviour> { target };
                _targetsTransformDict[targetType] = new List<Transform> { target.transform };
            }

            if (!_targetsTransform.Contains(target.transform))
                _targetsTransform.Add(target.transform);
        }
        
        public void AddTargetList(IEnumerable<MonoBehaviour> targets)
        {
            foreach (var target in targets)
            {
                AddTarget(target);
            }
        }

        private void AddTargetDict(Dictionary<Type, IEnumerable<MonoBehaviour>> targetsDict)
        {
            if (targetsDict == null) return;

            foreach (var kv in targetsDict)
            {
                var type = kv.Key;
                var enumerable = kv.Value;
                if (enumerable == null) continue;

                foreach (var target in enumerable)
                {
                    if (target == null) continue;
                    AddTarget(target);
                }
            }
        }
        
        public void AddTargetDict<T>(Dictionary<Type, List<T>> targetsDict) where T : MonoBehaviour
        {
            if (targetsDict == null) return;

            var converted = targetsDict.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.Cast<MonoBehaviour>() as IEnumerable<MonoBehaviour>
            );

            AddTargetDict(converted);
        }
        
        public List<T> GetTarget<T>(bool isNullLog = true) where T : MonoBehaviour
        {
            if (!_targetsDict.TryGetValue(typeof(T), out List<MonoBehaviour> targetTemp))
            {
                if(isNullLog)
                    DevLog.LogError($"ScriptFinderSO: No targets found for type {typeof(T).Name}.");
                return null;
            }

            var result = new List<T>();
            foreach (var component in targetTemp)
            {
                if (component is T t) result.Add(t);
                else
                {
                    DevLog.LogError(
                        $"ScriptFinderSO: Target is not of type {typeof(T).Name}. " +
                        $"Current type: {component?.GetType().Name ?? "null"}");
                }
            }
            return result;
        }

        
        public List<T> GetInterface<T>(bool isNullLog = true) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                DevLog.LogError($"[ScriptFinderSO] Type {typeof(T).Name} is not an interface.\n" +
                                "This method requires an interface type. ");
                return null;
            }
            
            if (!_targetsDict.TryGetValue(typeof(T), out List<MonoBehaviour> targetTemp))
            {
                if(isNullLog)
                    DevLog.LogError($"ScriptFinderSO: No targets found for interface {typeof(T).Name}.");
                return null;
            }

            var result = new List<T>();

            foreach (var component in targetTemp)
            {
                if (component is T i)
                {
                    result.Add(i);
                }
                else
                {
                    DevLog.LogError(
                        $"ScriptFinderSO: Target does not implement interface {typeof(T).Name}. " +
                        $"Current type: {component?.GetType().Name ?? "null"}");
                }
            }

            return result;
        }
        
        public List<Transform> GetTransform<T>() => _targetsTransformDict[typeof(T)];
    }
}