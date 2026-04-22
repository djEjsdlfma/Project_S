using MoonLib.ScriptFinder_Pro.RunTime.DevLogs;
using UnityEngine;

namespace MoonLib.ScriptFinder_Pro.RunTime.Finder.OneFinder
{
    [CreateAssetMenu(fileName = "ScriptFinder",menuName = "ScriptFinder/Normal", order = 0)]
    public class ScriptFinderSO : ScriptOneFinderBase
    {
        public override void Initialize()
        {
            FinderType = FinderType.Normal;
        }
        
        public void SetTarget(MonoBehaviour target)
        {
            _target = target;
            _targetTransform = target?.transform;
        }
        
        public T GetTarget<T>(bool isNullLog = true) where T : MonoBehaviour
        {
            if (_target == null)
            {
                if (isNullLog)
                    DevLog.LogError($"ScriptFinderSO: Target is null when trying to get type {typeof(T).Name}");
                return null;
            }
            
            if (_target is T t) return t;
            DevLog.LogError($"ScriptFinderSO: Target is not of type {typeof(T).Name}. " +
                            $"Current type: {_target?.GetType().Name ?? "null"}");
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
            
            if (_target == null)
            {
                if (isNullLog)
                    DevLog.LogError($"ScriptFinderSO: Target is null when trying to get interface {typeof(T).Name}");
                return null;
            }
    
            if (_target is T i) return i;
            DevLog.LogError($"ScriptFinderSO: Target does not implement interface {typeof(T).Name}. " +
                            $"Current type: {_target.GetType().Name}");
            return null;
        }
        
        public Transform GetTransform() => _targetTransform;
        
        
    }
}