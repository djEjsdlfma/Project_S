
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
        private PlayerStatCore _playerStatCore;
        
        private UnityAction _dayChangeAction;
        
        private void Awake()
        {
            if(_playerStatCore == null)
                _playerStatCore = CoreHandler.Instance.GetCore<PlayerStatCore>();
            
            _closeBtn = GetComponent<Button>();
            _closeBtn.interactable = false;
            _dayChangeAction = (() => _playerStatCore.IncreaseDay());
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