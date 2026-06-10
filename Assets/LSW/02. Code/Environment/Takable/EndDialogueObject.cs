
using System.Collections.Generic;
using LSW._02._Code.System___Manager;
using LSW._02._Code.UI;
using UnityEngine;
using UnityEngine.Events;

namespace LSW._02._Code.Environment.Takable
{
    public class EndDialogueObject : MonoBehaviour, ITakable
    {
        [SerializeField] private List<string> wordList;
        [SerializeField] private UnityEvent<EndDialogueObject, List<string>> takePictureEvent;
        [SerializeField] private UnityEvent endDialogueEvent;
        
        private bool IsAlreadyTook { get; set; } = false;
        private ShowWordUISystem _showWordUISystem;

        private void Start()
        {
            _showWordUISystem = SystemManager.Instance.GetSystemManager<ShowWordUISystem>();
        }

        public void Take()
        {
            IsAlreadyTook = true;
            takePictureEvent?.Invoke(this, wordList);
            
            if(_showWordUISystem != null)
            {
                _showWordUISystem.OnEndShowWord += EndDialogue;
            }
        }

        public bool IsDisableCapture()
        {
            return true;
        }
        
        public bool CanBeTaken()
            => !IsAlreadyTook && _showWordUISystem.StartShowWord(wordList);

        private void EndDialogue(bool _)
        {
            _showWordUISystem.OnEndShowWord -= EndDialogue;
            endDialogueEvent?.Invoke();
        }

        private void OnDestroy()
        {
            takePictureEvent.RemoveAllListeners();
            IsAlreadyTook = false;
        }
    }
}