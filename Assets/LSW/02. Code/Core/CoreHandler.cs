
using System.Collections.Generic;
using LSW._02._Code.System___Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LSW._02._Code.Core
{
    [DefaultExecutionOrder(-102)]
    public class CoreHandler : MonoSingleton<CoreHandler>
    {
        private readonly List<ICore> _coreList = new List<ICore>();
        
        protected override void Awake()
        {
            base.Awake();
            
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += LoadScene;
            Initialize();
        }

        private void Initialize()
        {
            ICore[] managers = GetComponentsInChildren<ICore>();
            for (int i = 0; i < managers.Length; i++)
            {
                if(managers[i] == null)
                    continue;
                
                _coreList.Add(managers[i]);
            }
            
            _coreList.ForEach(core => core.Initialize(this));
            LoadScene(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
        
        private void LoadScene(Scene scene, LoadSceneMode _)
        {
            SceneType sceneType = (SceneType)scene.buildIndex;
                
            for (int i = 0; i < _coreList.Count; i++)
            {
                _coreList[i].LoadScene(sceneType);
            }
        }

        public T GetCore<T>() where T : class, ICore
        {
            return _coreList.Find(x => x is T) as T;
        }

        public void ResetCoreAllData()
        {
            for (int i = 0; i < _coreList.Count; i++)
            {
                _coreList[i].Reset();
            }
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            SceneManager.sceneLoaded -= LoadScene;
            ResetCoreAllData();
        }
    }
    
    public enum SceneType
    {
        None = -1,
        DataLoadScene = 0,
        MainTabletScene = 1,
        LeeJaeYoonScene = 2,
        DaEunJungScene = 3,
        YulParkScene = 4,
        SeoAhYoonScene = 5,
        ChoiMyeongJinScene = 6
    }
}