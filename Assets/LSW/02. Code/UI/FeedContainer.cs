
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
            _feeds = GetComponentsInChildren<Feed>();
        }

        private void Start()
        {
            _gameStatueCore = CoreHandler.Instance.GetCore<GameStatueCore>();
            
            if(_gameStatueCore == null || _feeds.Length <= 0)
                return;

            for (int i = 0; i < _gameStatueCore.CurrentDay - 1; i++)
            {
                _feeds[0].OnUploadClickImmediately();
                // 이거 수정 예정(일단 이재윤만 하는 걸로)
            }
            
            gameObject.SetActive(false);
        }
    }
}