using System;
using LSW._02._Code.Core;
using LSW._02._Code.Core.Cores;
using LSW._02._Code.System___Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LSW._02._Code.UI
{
    public class PadTalkHotBarBtn : MonoBehaviour
    {
        [SerializeField] private ChatProfileContainer chatProfileContainer;
        [SerializeField] private Image alarmImage;
        
        private Button _button;
        private BubbleManager _bubbleManager;
        private UnityAction _action;
        
        private void Awake()
        {
            _bubbleManager = SystemManager.Instance.GetSystemManager<BubbleManager>();
            _button = GetComponent<Button>();
            
            if(_button == null || chatProfileContainer == null || _bubbleManager == null)
                return;
            
            _action = () =>
            {
                chatProfileContainer.EnableAllProfile();
                _bubbleManager.EnableInteract();
                _bubbleManager.ChangeGuestDialogue(Guest.None);
            };
            _button.onClick.AddListener(_action);
        }

        private void Start()
        {
            if (_bubbleManager != null)
            {
                _bubbleManager.onAlarmStateChanged += HandleAlarmStateChanged;
                UpdateAlarmImage();
            }
        }

        private void HandleAlarmStateChanged(Guest guest, bool state)
        {
            UpdateAlarmImage();
        }

        private void UpdateAlarmImage()
        {
            if (alarmImage != null && chatProfileContainer != null && _bubbleManager != null)
            {
                alarmImage.gameObject.SetActive(chatProfileContainer.HasAnyUnread(_bubbleManager));
            }
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(_action);

            if (_bubbleManager != null)
                _bubbleManager.onAlarmStateChanged -= HandleAlarmStateChanged;
        }
    }
}