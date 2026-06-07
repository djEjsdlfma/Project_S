using System.Collections.Generic;
using UnityEngine;

namespace LSW._02._Code.System___Manager
{
    [DefaultExecutionOrder(-101)]
    public class SystemManager : MonoSingleton<SystemManager>
    {
        private readonly List<ISystemManager> _managerList = new List<ISystemManager>();
        private bool _isInitialized = false;

        protected void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            ISystemManager[] managers = GetComponentsInChildren<ISystemManager>();
            for (int i = 0; i < managers.Length; i++)
            {
                if (managers[i] == null) continue;
                _managerList.Add(managers[i]);
                managers[i].Initialize(this);
            }
            _isInitialized = true;
        }

        public T GetSystemManager<T>() where T : class, ISystemManager
        {
            if (!_isInitialized)
            {
                Initialize();
            }
        
            return _managerList.Find(x => x is T) as T;
        }
        
        protected override void OnDestroy()
        {
            for (int i = 0; i < _managerList.Count; i++)
            {
                _managerList[i].Reset();
            }
            _managerList.Clear();
            _isInitialized = false;
            
            base.OnDestroy();
        }
    }
}