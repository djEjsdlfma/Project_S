using System;
using System.Collections.Generic;
using System.Linq;
using MoonLib.ScriptFinder_Pro.RunTime.DevLogs;
using MoonLib.ScriptFinder_Pro.RunTime.Finder;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.ListFinder;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.OneFinder;
using UnityEngine;

namespace MoonLib.ScriptFinder_Pro.RunTime.Manager
{
    [DefaultExecutionOrder(-10)]
    public class FinderManager : MonoBehaviour
    {
        [SerializeField] private ScriptFinderBase[] finders;

        private void Awake()
        {
            if (finders == null || finders.Length == 0) return;
            Initialize();
        }

        private void Initialize()
        {
            List<ScriptFinderSO> finderList = new List<ScriptFinderSO>();
            List<ScriptAllFinderSO> allFinders = new List<ScriptAllFinderSO>();
            List<ScriptListFinderSO> scriptListFinders = new List<ScriptListFinderSO>();
            List<ScriptListAllFinderSO> scriptListAllFinders = new List<ScriptListAllFinderSO>();

            if (finders == null || finders.Length == 0) return;

            foreach (var finder in finders)
            {
                if (finder == null) continue;
                finder.Initialize();
                switch (finder.FinderType)
                {
                    case FinderType.None:
                        continue;

                    case FinderType.Normal:
                        if (finder is ScriptFinderSO s) finderList.Add(s);
                        break;

                    case FinderType.All:
                        if (finder is ScriptAllFinderSO a) allFinders.Add(a);
                        break;

                    case FinderType.List:
                        if (finder is ScriptListFinderSO l) scriptListFinders.Add(l);
                        break;

                    case FinderType.ListAll:
                        if (finder is ScriptListAllFinderSO la) scriptListAllFinders.Add(la);
                        break;
                }
            }
            MonoBehaviour[] components;

#if UNITY_6000_4_OR_NEWER 
            // 1. unity 6.4 (6000.4.x) Or above
            components = FindObjectsByType<MonoBehaviour>();
            
#elif UNITY_2022_3_OR_NEWER
            // 2. unity 2023.x or above
            components = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            
#else
            // 3. other version (2019, 2020, 2021 or any)
            components = FindObjectsOfType<MonoBehaviour>();
            
#endif
            
            if (components.Length == 0)
            {
                DevLog.LogWarning("components is nothing this scene");
                return;
            }

            foreach (var finder in finderList)
            {
                if (finder == null) continue;
                Type keyType = finder.KeyType?.Type;
                if (keyType == null) continue;

                var component = finder.IsFindChild
                    ? components.FirstOrDefault(c => keyType.IsInstanceOfType(c))
                    : components.FirstOrDefault(c => c.GetType() == keyType);

                if (component != null)
                {
                    finder.SetTarget(component);
                }
            }

            foreach (var finder in allFinders)
            {
                if (finder == null) continue;
                Type keyType = finder.KeyType?.Type;
                if (keyType == null) continue;

                var matched = finder.IsFindChild
                    ? components.Where(c => keyType.IsInstanceOfType(c)).ToList()
                    : components.Where(c => c.GetType() == keyType).ToList();

                if (matched.Count > 0)
                {
                    finder.SetTarget(matched);
                }
            }

            foreach (var finder in scriptListFinders)
            {
                if (finder == null) continue;
                Dictionary<Type, MonoBehaviour> target = new();
                if (finder.KeyType == null) continue;

                foreach (var keyTypeRef in finder.KeyType)
                {
                    if (keyTypeRef == null) continue;
                    Type type = keyTypeRef.SType.Type;
                    if (type == null) continue;

                    MonoBehaviour component;

                    if (finder.IsFindChild || keyTypeRef.IsFindChild)
                        component = components.FirstOrDefault(c => type.IsInstanceOfType(c));
                    else
                        component = components.FirstOrDefault(c => c.GetType() == type);

                    if (component != null)
                    {
                        target.TryAdd(type, component);
                    }
                }

                finder.SetTarget(target);
            }

            foreach (var finder in scriptListAllFinders)
            {
                if (finder == null) continue;
                Dictionary<Type, List<MonoBehaviour>> target = new();
                if (finder.KeyType == null) continue;

                foreach (var keyTypeRef in finder.KeyType)
                {
                    if (keyTypeRef == null) continue;
                    Type type = keyTypeRef.SType.Type;
                    if (type == null) continue;

                    List<MonoBehaviour> matched;
                    
                    if(finder.IsFindChild || keyTypeRef.IsFindChild)
                        matched = components.Where(c => type.IsInstanceOfType(c)).ToList();
                    else
                        matched = components.Where(c => c.GetType() == type).ToList();

                    if (matched.Count > 0)
                    {
                        target[type] = matched;
                    }
                }

                finder.SetTarget(target);
            }
        }

    }
}