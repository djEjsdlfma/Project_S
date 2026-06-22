
using System;
using LSW._02._Code.Core;
using LSW._02._Code.Core.Cores;
using UnityEngine;

namespace LSW._02._Code.UI
{
    public class FeedContainer : MonoBehaviour
    {
        private Feed[] _feeds;
        private GameStatueCore _gameStatueCore;

        private void Awake()
        {
            if(_feeds == null)
                _feeds = GetComponentsInChildren<Feed>();
            
            Initialize();
            gameObject.SetActive(false);
        }

        private void Initialize()
        {
            _gameStatueCore = CoreHandler.Instance.GetCore<GameStatueCore>();
            
            if(_gameStatueCore == null || _feeds.Length <= 0)
                return;

            for (int i = 0; i < _gameStatueCore.CurrentDay - 1; i++)
            {
                _feeds[(_gameStatueCore.CurrentDay % 5) - 1].OnUploadClickImmediately();
            }
        }
    }
}