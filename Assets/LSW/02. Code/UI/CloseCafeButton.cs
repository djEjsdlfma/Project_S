
using System;
using LSW._02._Code.Core;
using LSW._02._Code.Core.Cores;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LSW._02._Code.UI
{
    public class CloseCafeButton : MonoBehaviour
    {
        private Button _closeBtn;
        private GameStatueCore _gameStatueCore;
        
        private UnityAction _dayChangeAction;
        
        private void Awake()
        {
            if(_gameStatueCore == null)
                _gameStatueCore = CoreHandler.Instance.GetCore<GameStatueCore>();
            
            _closeBtn = GetComponent<Button>();
            _closeBtn.interactable = false;
            _dayChangeAction = (() => _gameStatueCore.IncreaseDay());
        }

        public void EnableInteractable()
        {
            _closeBtn.interactable = true;
            _closeBtn.onClick.AddListener(_dayChangeAction);
        }

        private void OnDestroy()
        {
            _closeBtn.onClick.RemoveAllListeners();
        }
    }
}