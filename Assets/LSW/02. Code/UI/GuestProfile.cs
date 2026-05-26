using LSW._02._Code.Core;
using LSW._02._Code.Core.Cores;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Code.UI
{
    public class GuestProfile : MonoBehaviour
    {
        [SerializeField] private Image currentStatueImage;
        [SerializeField] private Image alarmImage;
        [SerializeField] private TextMeshProUGUI lastMessageText;
        [SerializeField] private TextMeshProUGUI lastOnlineText;
        
        [field: SerializeField] public Guest Guest { get; private set; }

        private bool _isOpenedChat = false;

        private void Awake()
        {
            if (Guest == Guest.None)
            {
                var btn = GetComponentInChildren<GuestProfileSelectBtn>();
                if (btn != null)
                {
                    Guest = btn.Guest;
                }
            }
            
            int currentDay = CoreHandler.Instance.GetCore<GameStatueCore>().CurrentDay;
            int targetGuestIndex = currentDay % 5 == 0 ? 5 : currentDay % 5;
            bool isDialogueDay = (targetGuestIndex == (int)Guest);
            int lastOnlineDay = Mathf.Abs(currentDay - (((currentDay - (int)Guest) % 5 + 5) % 5));
            
            lastOnlineText.SetText(isDialogueDay ? "온라인" : $"최근 접속 {lastOnlineDay}일 전");
            currentStatueImage.color = isDialogueDay ? Color.green : Color.gray;
        }

        public void OpenChat(bool isOpened = true)
        {
            _isOpenedChat = true;
        }
        
        public void SetProfile(string lastMessage, bool hasAlarm)
        {
            if (lastMessage != null)
                lastMessageText.SetText(lastMessage);
            
            if(!_isOpenedChat)
                alarmImage.gameObject.SetActive(hasAlarm);
        }
    }
}