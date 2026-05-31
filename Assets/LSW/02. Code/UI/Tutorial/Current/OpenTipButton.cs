
using System;
using LSW._02._Code.System___Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LSW._02._Code.UI.Tutorial.Current
{
	[RequireComponent(typeof(Button))]
    public class OpenTipButton : MonoBehaviour
    {
        [SerializeField] private HelpUI helpUI;
        
        private Title _title;
        private Button _button;
        private UnityAction _btnClickAction;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _title = SystemManager.Instance.GetSystemManager<Title>();

            _btnClickAction = () =>
            {
                helpUI.gameObject.SetActive(true);
                if(_title != null)
                    _title.canEnterPassword = false;
            };
            _button.onClick.AddListener(_btnClickAction);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(_btnClickAction);
        }
    }
}