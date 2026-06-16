using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LSW._02._Code.System___Manager;
using TMPro;
using UnityEngine;

namespace LSW._02._Code.UI
{
    public class ShowWordUISystem : MonoBehaviour, ISystemManager
    {
        [SerializeField] private TextMeshProUGUI wordUIText;
        [SerializeField] private float textShowDuration;
        [SerializeField] private float textMaintainDuration;
        [SerializeField] private float textHideDuration;
        
        private bool _canShowWord = true;
        private WaitForSeconds _mainTainSec;
        
        public event Action<bool> OnEndShowWord;

        public void Initialize(SystemManager systemManager)
        {
            _mainTainSec = new WaitForSeconds(textMaintainDuration);
        }

        public bool StartShowWord(List<string> words)
        {
            if (!_canShowWord) 
                return false;
    
            Debug.Log("Start Show Word:");
            
            _canShowWord = false;
            StartCoroutine(ShowWordsSequence(words));
            wordUIText.SetText(string.Empty);
            return true;
        }
        
        public bool CanShowWord() => _canShowWord;

        private IEnumerator ShowWordsSequence(List<string> words)
        {
            if (words.Count == 0)
            {
                OnEndShowWord?.Invoke(false);
                _canShowWord = true;
                yield break;
            }
            
            foreach (string word in words)
            {
                wordUIText.SetText(word);
                wordUIText.alpha = 0;
                
                yield return wordUIText.DOFade(1, textShowDuration).WaitForCompletion();
                yield return _mainTainSec;
                yield return wordUIText.DOFade(0, textHideDuration).WaitForCompletion();
            }
            
            OnEndShowWord?.Invoke(true);
            _canShowWord = true;
        }

        public void Reset() { }
    }
}