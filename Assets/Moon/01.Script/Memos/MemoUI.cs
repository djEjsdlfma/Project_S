using System;
using AYellowpaper.SerializedCollections;
using Moon._01.Script.Datas;
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

        private bool _isDays = false;

        public void SetMemo(Human human, string mText,int day , MemoSystem memoSystem)
        {
            _memoSystem = memoSystem;
            humanText.text = _memoSystem.memoDict[human];
            _human = human;
            memoText.text = mText;

            if (!_isDays)
            {
                dayText.text = $"Day : {day}";
                _isDays = true;
            }
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