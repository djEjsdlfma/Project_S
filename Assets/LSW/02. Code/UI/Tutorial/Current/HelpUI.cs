using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Code.UI.Tutorial.Current
{
    public class HelpUI : MonoBehaviour
    {
        [SerializeField] private HelpListButton helpListButton;
        [SerializeField] private RectTransform content;
        [SerializeField] private Image helpImage;
        [SerializeField] private TextMeshProUGUI helpText;
        [SerializeField] private TextMeshProUGUI pageText;
        [SerializeField] private List<HelpData> helpDataList;

        [SerializeField] private Image[] leftBtn;
        [SerializeField] private Image[] rightBtn;

        private List<HelpUIData> _currentHelpDataList;
        private List<HelpListButton> _helpListButtons = new List<HelpListButton>();
        private int _currentHelpIndex = -1;

        private void Start()
        {
            foreach (var data in helpDataList)
            {
                HelpListButton helpUI = Instantiate(helpListButton, content);
                helpUI.Initialize(this, data.helpBtnName, data.helpUIData);
                _helpListButtons.Add(helpUI);
            }
        }

        private void Update()
        {
            if (_currentHelpDataList == null)
                return;

            if (Input.GetKeyDown(KeyCode.Q))
                MoveToIndex(_currentHelpIndex - 1);
            if (Input.GetKeyDown(KeyCode.E))
                MoveToIndex(_currentHelpIndex + 1);

            
        }

        public void SetHelpUI(List<HelpUIData> helpUIData)
        {
            if (_helpListButtons.Count > 0)
            {
                foreach (var btn in _helpListButtons)
                    btn.OtherHelpListButtonClick();
            }
            
            _currentHelpDataList = helpUIData;
            _currentHelpIndex = 0;
            UpdateDisplay();
        }

        private void MoveToIndex(int moveIndex)
        {
            if (_currentHelpDataList == null || moveIndex < 0 || moveIndex >= _currentHelpDataList.Count)
                return;

            _currentHelpIndex = moveIndex;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if(_currentHelpIndex == 0)
                for (int i = 0; i < 2; i++) leftBtn[i].DOFade(0.75f, 0f);
            else
                for (int i = 0; i < 2; i++) leftBtn[i].DOFade(1f, 0f);

            if (_currentHelpIndex >= _currentHelpDataList.Count - 1)
                for (int i = 0; i < 2; i++) rightBtn[i].DOFade(0.75f, 0f);
            else
                for (int i = 0; i < 2; i++) rightBtn[i].DOFade(1f, 0f);


            var data = _currentHelpDataList[_currentHelpIndex];
            helpImage.sprite = data.helpImage;
            helpImage.preserveAspect = true;
            helpText.text = data.helpText;
            pageText.SetText($"{_currentHelpIndex + 1} / {_currentHelpDataList.Count} page");
        }
    }

    [Serializable]
    public struct HelpData
    {
        public string helpBtnName;
        public List<HelpUIData> helpUIData;
    }

    [Serializable]
    public struct HelpUIData
    {
        public Sprite helpImage;
        [TextArea]
        public string helpText;
    }
}