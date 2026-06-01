
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Code.UI.Tutorial.Current
{
    [RequireComponent(typeof(Button))]
    public class HelpListButton : MonoBehaviour
    {
        [SerializeField] private Image colorImage;
        [SerializeField] private TextMeshProUGUI helpNameText;
        
        private bool _wasInitialized = false;
        private List<HelpUIData> _helpUIData;
        
        private HelpUI _helpUI;
        private Button _button;
        private event Action<List<HelpUIData>> OnHelpListButtonClicked;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            colorImage = GetComponent<Image>();
            
            OtherHelpListButtonClick();
            _button.onClick.AddListener(HelpListButtonClick);
        }

        public void Initialize(HelpUI helpUI, string helpName, List<HelpUIData> helpUIData)
        {
            if(_wasInitialized)
                return;
            
            helpNameText.SetText(helpName);
            _helpUIData = helpUIData;
            _wasInitialized = true;
            
            _helpUI = helpUI;
            OnHelpListButtonClicked += helpUI.SetHelpUI;
        }

        private void HelpListButtonClick()
        {
            if(!_wasInitialized)
                return;
            
            OnHelpListButtonClicked?.Invoke(_helpUIData);
            colorImage.color = Color.white;
        }

        public void OtherHelpListButtonClick()
        {
            colorImage.color = Color.gray;
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
            if (_helpUI != null)
                OnHelpListButtonClicked -= _helpUI.SetHelpUI;
        }
    }
}