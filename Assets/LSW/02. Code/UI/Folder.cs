using System;
using LSW._02._Code.Core.Cores;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LSW._02._Code.UI
{
    public class Folder : MonoBehaviour
    {
        [SerializeField] private PhotoUI photoUI;
        [SerializeField] private Guest guest;

        private Button _button;
        private UnityAction _onClickEvent;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _onClickEvent = () => photoUI.ShowMyPhoto(guest);
            _button.onClick.AddListener(_onClickEvent);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(_onClickEvent);
        }
    }
}