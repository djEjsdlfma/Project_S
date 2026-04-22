using System.Collections.Generic;
using MoonLib.ScriptFinder_Pro.RunTime.DevLogs;
using UnityEngine;

namespace MoonLib.ScriptFinder_Pro.RunTime.Finder.OneFinder
{
    [CreateAssetMenu(fileName = "ScriptAllFinder",menuName = "ScriptFinder/All", order = 1)]
    public class ScriptAllFinderSO : ScriptOneFinderBase
    {

        public override void Initialize()
        {
            FinderType = FinderType.All;
        }
        
        public void SetTarget(List<MonoBehaviour> targets)
        {
            _targets = targets;
            _targetsTransform = targets.ConvertAll(t => t.transform);
        }
        
        public void AddTargetList(IEnumerable<MonoBehaviour> targets)
        {
            foreach (var target in targets)
            {
                AddTarget(target);
            }
        }

        public void AddTarget(MonoBehaviour target)
        {
            if (target == null)
            {
                DevLog.LogError("ScriptFinderSO: Target is null or destroyed.");
                return;
            }
            
            if(target.GetType() == KeyType.Type)
            {
                _targets.Add(target);
                _targetsTransform.Add(target.transform);
            }
            else
            {
                DevLog.LogError($"ScriptFinderSO: Target is not of type {KeyType.Type.Name}. " +
                                $"Current type: {target.GetType().Name}");
            }
        }
        
        public List<T> GetTarget<T>(bool isNullLog = true) where T : MonoBehaviour
        {
            if (_targets.Count > 0)
            {
                List<T> result = new List<T>();
                foreach (var component in _targets)
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
            if(isNullLog)
                DevLog.LogError($"ScriptFinderSO: No targets found for type {typeof(T).Name}.");
            return null;
        }
        
        public List<T> GetInterface<T>(bool isNullLog = true) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                DevLog.LogError($"[ScriptFinderSO] Type {typeof(T).Name} is not an interface.\n" +
                                "This method requires an interface type. ");
                return null;
            }
            
            if (_targets.Count == 0)
            {
                if(isNullLog)
                    DevLog.LogError($"ScriptFinderSO: No targets found for interface {typeof(T).Name}.");
                return null;
            }

            List<T> result = new List<T>();

            foreach (var component in _targets)
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
        
        public List<Transform> GetTransform() => _targetsTransform;
    }
}