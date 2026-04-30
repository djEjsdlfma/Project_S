using System.Collections.Generic;
using UnityEngine;

namespace LSW._02._Code.System___Manager
{
    [DefaultExecutionOrder(-100)]
    public class SystemManager : MonoSingleton<SystemManager>
    {
        private readonly List<ISystemManager> _managerList = new List<ISystemManager>();

        protected override void Awake()
        {
            base.Awake();
            Initialize();
            // SceneManager.sceneUnloaded += LoadScene;
        }

        private void Initialize()
        {
            ISystemManager[] managers = GetComponentsInChildren<ISystemManager>();
            for (int i = 0; i < managers.Length; i++)
            {
                if(managers[i] == null)
                    continue;
                
                _managerList.Add(managers[i]);
                managers[i].Initialize(this);
            }
            // LoadScene(SceneManager.GetActiveScene());
        }
        
        public T GetSystemManager<T>() where T : class, ISystemManager
        {
            return _managerList.Find(x => x is T) as T;
        }
        
        // private void LoadScene(Scene scene)
        // {
        //     SceneType sceneType = (SceneType)scene.buildIndex;
        //         
        //     for (int i = 0; i < _managerList.Count; i++)
        //     {
        //         _managerList[i].LoadScene(sceneType);
        //     }
        // }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            for (int i = 0; i < _managerList.Count; i++)
            {
                _managerList[i].Reset();
            }
            // SceneManager.sceneUnloaded -= LoadScene;
        }
    }
    
    public enum SceneType
    {
        None = -1,
        StartScene = 0,
        MainGame = 1
    }
}