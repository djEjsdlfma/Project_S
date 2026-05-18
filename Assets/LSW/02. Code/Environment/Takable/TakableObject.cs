
using System.Collections.Generic;
using LSW._02._Code.System___Manager;
using LSW._02._Code.UI;
using UnityEngine;
using UnityEngine.Events;

namespace LSW._02._Code.Environment.Takable
{
    public class TakableObject : MonoBehaviour, ITakable
    {
        [SerializeField] private List<string> wordList;
        [SerializeField] private UnityEvent<TakableObject, List<string>> takePictureEvent;
        
        private bool IsAlreadyTook { get; set; } = false;
        private ShowWordUISystem _showWordUISystem;

        private void Awake()
        {
            _showWordUISystem = SystemManager.Instance.GetSystemManager<ShowWordUISystem>();
        }

        public void Take()
        {
            if(IsAlreadyTook)
                return;
            
            if(!_showWordUISystem.StartShowWord(wordList))
                return;
            
            IsAlreadyTook = true;
            takePictureEvent?.Invoke(this, wordList);
        }

        private void OnDestroy()
        {
            takePictureEvent.RemoveAllListeners();
            IsAlreadyTook = false;
        }
    }
}