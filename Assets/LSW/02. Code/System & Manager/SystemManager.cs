using System.Collections.Generic;
using UnityEngine;

namespace LSW._02._Code.System___Manager
{
    [DefaultExecutionOrder(-101)]
    public class SystemManager : MonoSingleton<SystemManager>
    {
        private readonly List<ISystemManager> _managerList = new List<ISystemManager>();

        protected override void Awake()
        {
            base.Awake();
            Initialize();
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
        }
        
        public T GetSystemManager<T>() where T : class, ISystemManager
        {
            return _managerList.Find(x => x is T) as T;
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            for (int i = 0; i < _managerList.Count; i++)
            {
                _managerList[i].Reset();
            }
        }
    }
}