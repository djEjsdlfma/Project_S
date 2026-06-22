
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
            
            gameObject.SetActive(false);
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _gameStatueCore = CoreHandler.Instance.GetCore<GameStatueCore>();
            
            if(_gameStatueCore == null || _feeds.Length <= 0)
                return;

            for (int i = 0; i < Mathf.Min(_gameStatueCore.CurrentDay - 1, _feeds.Length); i++)
            {
                _feeds[i].OnUploadClickImmediately();
            }
        }
    }
}