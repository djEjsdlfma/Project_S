using System;
using LSW._02._Code.Core;
using LSW._02._Code.Core.Cores;
using LSW._02._Code.System___Manager;
using LSW._02._Code.UI;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.OneFinder;
using UnityEngine;

namespace LSW._02._Code.Environment.Takable
{
    public class EndDialogueObjectContainer : MonoBehaviour
    {
        [SerializeField] private ScriptFinderSO playerFinder;
        [SerializeField] private int dialogueEndCount;
        
        private ShowWordUISystem _wordUISystem;
        private Transition _transition;
        
        private EndDialogueObject[] _endDialogueObjects;
        private Player.Player _player;
        private int _endDialogueCount;
        
        private void Start()
        {
            _wordUISystem = SystemManager.Instance.GetSystemManager<ShowWordUISystem>();
            _transition = CoreHandler.Instance.GetCore<Transition>();
            _player = playerFinder.GetTarget<Player.Player>();

            _endDialogueCount = dialogueEndCount;
            _wordUISystem.OnEndShowWord += EndShowWord;
        }

        private void EndShowWord(bool isEnd)
        {
            if (!isEnd)
                return;

            _endDialogueCount--;

            if (_endDialogueCount <= 0)
            {
                if (_player != null)
                    _player.SetStop(true);
                _transition.TransitionScene(SceneType.MainTabletScene, TransitionType.LeaveToPlatform);
            }
        }

        private void OnDestroy()
        {
            _wordUISystem.OnEndShowWord -= EndShowWord;
        }
    }
}