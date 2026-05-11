
using LSW._02._Code.Core.Cores;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LSW._02._Code.UI
{
    public class GuestProfileSelectBtn : MonoBehaviour
    {
        [SerializeField] private BubbleManager bubbleManager;
        [SerializeField] private Guest guest;
        
        private Button _button;
        private UnityAction _action;

        private void Awake()
        {
            _button = GetComponent<Button>();
            if (_button != null && guest != Guest.None && bubbleManager != null)
            {
                _action = (() => bubbleManager.ChangeGuestDialogue(guest));
                _button.onClick.AddListener(_action);
            }
        }

        private void OnDestroy()
        {
            if (_button != null && guest != Guest.None && bubbleManager != null)
            {
                _button.onClick.RemoveListener(_action);
            }
        }
    }
}