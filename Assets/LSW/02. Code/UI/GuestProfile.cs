using LSW._02._Code.Core;
using LSW._02._Code.Core.Cores;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Code.UI
{
    public class GuestProfile : MonoBehaviour
    {
        [SerializeField] private ChattingUIManager chattingUIManager;
        [SerializeField] private Image currentStatueImage;
        [SerializeField] private Image alarmImage;
        [SerializeField] private TextMeshProUGUI lastMessageText;
        [SerializeField] private TextMeshProUGUI onlineOpenedText;
        [SerializeField] private TextMeshProUGUI lastOnlineText;
        
        [field: SerializeField] public Guest Guest { get; private set; }
        public bool IsActivable { get; set; } = false;
        
        public bool IsOpenedChat { get; private set; } = false;


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
        }

        private void Start()
        {
            int currentDay = CoreHandler.Instance.GetCore<GameStatueCore>().CurrentDay;
            int targetGuestIndex = currentDay % 5 == 0 ? 4 : currentDay % 5;
            bool isDialogueDay = (targetGuestIndex == (int)Guest);
            int lastOnlineDay = Mathf.Abs(currentDay - (int)Guest);
            
            lastOnlineText.SetText(isDialogueDay ? "온라인" : $"최근 접속 {lastOnlineDay}일 전");
            currentStatueImage.color = isDialogueDay ? Color.green : Color.gray;
            alarmImage.gameObject.SetActive(isDialogueDay);
        }

        public void OpenChat(bool isOpened = true)
        {
            chattingUIManager.DisableChat(Guest, isOpened);
            IsOpenedChat = isOpened;
            lastMessageText.gameObject.SetActive(!isOpened);
            onlineOpenedText.gameObject.SetActive(isOpened);
            lastOnlineText.gameObject.SetActive(!isOpened);
        }
        
        public void SetProfile(string lastMessage, bool hasAlarm)
        {
            if (lastMessage != null)
                lastMessageText.SetText(lastMessage);
            
            if(!IsOpenedChat)
                alarmImage.gameObject.SetActive(hasAlarm);
        }
    }
}