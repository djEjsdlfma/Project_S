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

        public void Initialize(SystemManager systemManager)
        {
            _mainTainSec = new WaitForSeconds(textMaintainDuration);
        }

        public bool StartShowWord(List<string> words)
        {
            if (!_canShowWord) 
                return false;
    
            _canShowWord = false;
            StartCoroutine(ShowWordsSequence(words));
            wordUIText.SetText(string.Empty);
            return true;
        }

        private IEnumerator ShowWordsSequence(List<string> words)
        {
            foreach (string word in words)
            {
                wordUIText.SetText(word);
                wordUIText.alpha = 0;
                
                yield return wordUIText.DOFade(1, textShowDuration).WaitForCompletion();
                yield return _mainTainSec;
                yield return wordUIText.DOFade(0, textHideDuration).WaitForCompletion();
            }
    
            _canShowWord = true;
        }

        public void Reset() { }
    }
}