
using LSW._02._Code.Core.Cores;
using LSW._02._Code.System___Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LSW._02._Code.UI
{
    public class GuestProfileSelectBtn : MonoBehaviour
    {
        [SerializeField] private GuestProfile parent;
        [SerializeField] private ChatProfileContainer profileContainer;
        [SerializeField] private Guest guest;
        [SerializeField] private Image alarmImage;

        public Guest Guest => guest;
        
        private BubbleManager _bubbleManager;
        private Button _button;
        private UnityAction _action;

        private void Awake()
        {
            _bubbleManager = SystemManager.Instance.GetSystemManager<BubbleManager>();
            _button = GetComponent<Button>();
            if (_button != null && guest != Guest.None && _bubbleManager != null)
            {
                _action = (() => 
                        {
                            profileContainer.DisableAllProfile(parent);
                            _bubbleManager.ChangeGuestDialogue(guest);
                            alarmImage.gameObject.SetActive(false);
                        }
                );
                _button.onClick.AddListener(_action);
            }
        }

        private void OnDestroy()
        {
            if (_button != null && guest != Guest.None && _bubbleManager != null)
            {
                _button.onClick.RemoveListener(_action);
            }
        }
    }
}