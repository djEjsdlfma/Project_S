using System.Collections.Generic;
using LSW._02._Code.Data;
using LSW._02._Code.System___Manager;
using UnityEngine.SceneManagement;

namespace LSW._02._Code.Core
{
    public class CoreHandler : MonoSingleton<CoreHandler>
    {
        private readonly List<ICore> _coreList = new List<ICore>();
        
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            Initialize();
            SceneManager.sceneUnloaded += LoadScene;
        }

        private void Initialize()
        {
            ICore[] managers = GetComponentsInChildren<ICore>();
            for (int i = 0; i < managers.Length; i++)
            {
                if(managers[i] == null)
                    continue;
                
                _coreList.Add(managers[i]);
                managers[i].Initialize(this);
            }
            LoadScene(SceneManager.GetActiveScene());
        }
        
        private void LoadScene(Scene scene)
        {
            SceneType sceneType = (SceneType)scene.buildIndex;
                
            for (int i = 0; i < _coreList.Count; i++)
            {
                _coreList[i].LoadScene(sceneType);
            }
        }

        public List<IDataLoadManager> GetDataLoadManagers()
        {
            List<IDataLoadManager> dataLoadManagers = new List<IDataLoadManager>();
            foreach (var manager in _coreList)
            {
                if (manager is IDataLoadManager)
                {
                    dataLoadManagers.Add(manager as IDataLoadManager);
                }
            }

            return dataLoadManagers;
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            SceneManager.sceneUnloaded -= LoadScene;
        }
    }
    
    public enum SceneType
    {
        None = -1,
        StartScene = 0,
        MainTabletScene = 1,
        PlatformerScene = 2,
    }
}