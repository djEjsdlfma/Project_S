using System;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Moon._01.Script.Memos
{
    public class MemoUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI humanText;
        [SerializeField] private TextMeshProUGUI memoText;
        [SerializeField] private TextMeshProUGUI dayText;
        private MemoSystem _memoSystem;

        private Human _human;

        public Human Human => _human;
        
        public string Text => memoText.text;
        public int Num { get; set; }

        public UnityEvent<MemoUI> clicked;
        
        public void SetMemo(Human human, string mText, MemoSystem memoSystem)
        {
            _memoSystem = memoSystem;
            humanText.text = _memoSystem.memoDict[human];
            _human = human;
            memoText.text = mText;
        }

        public void Click()
        {
            clicked?.Invoke(this);
        }

        private void OnDestroy()
        {
            clicked.RemoveAllListeners();
        }
    }
}