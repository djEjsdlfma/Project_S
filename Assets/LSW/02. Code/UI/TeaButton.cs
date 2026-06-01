
using System;
using LSW._02._Code.Core;
using LSW._02._Code.Core.Cores;
using LSW._02._Code.System___Manager;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Code.UI
{
    
    [RequireComponent(typeof(Button))]
    public class TeaButton : MonoBehaviour
    {
        private Button _button;
        
        private Transition _transition;
        private GameStatueCore _gameStatueCore;
        private BubbleManager _bubbleManager;
        
        private void Awake()
        {
            _bubbleManager = SystemManager.Instance.GetSystemManager<BubbleManager>();
            _button = GetComponent<Button>();
            
            _button.onClick.AddListener(() => _transition.TransitionScene(GetPlatformScene(), TransitionType.LeaveToPlatform));
        }
        
        private void Start()
        {
            _transition = CoreHandler.Instance.GetCore<Transition>();
            _gameStatueCore = CoreHandler.Instance.GetCore<GameStatueCore>();
        }

        private SceneType GetPlatformScene()
            => (SceneType)((_gameStatueCore.CurrentDay + 1) % 5);

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}