
using System;
using LSW._02._Code.Core;
using LSW._02._Code.Core.Cores;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Code.UI
{
    [RequireComponent(typeof(Button))]
    public class CandleButton : MonoBehaviour
    {
        private GameStatueCore _gameStatueCore;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void Start()
        {
            _gameStatueCore = CoreHandler.Instance.GetCore<GameStatueCore>();
            _button.onClick.AddListener(_gameStatueCore.IncreaseDayDebug);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}