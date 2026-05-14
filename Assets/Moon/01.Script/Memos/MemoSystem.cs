using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Moon._01.Script.Datas;
using TMPro;
using UnityEngine;

namespace Moon._01.Script.Memos
{
    public enum Human
    {
        LeeJaeYoon,
        NaSohee,
        ParkYul,
        YunSeo,
        ChoiMyungjin,
        None
    }

    [Serializable]
    public struct Memo
    {
        public Human human;
        public string text;
        [NonSerialized]public int num;
    }
    
    [Serializable]
    public class MemoWrapper
    {
        public List<Memo> memos = new List<Memo>();
    }
    
    public class MemoSystem : MonoBehaviour
    {
        [HideInInspector,SerializeField]private MemoWrapper memo = new MemoWrapper();

        [Header("Create")]
        [SerializeField] private GameObject prefab;
        [SerializeField] private Transform tParent;
        [SerializeField] private GameObject createPanel;
        [SerializeField] private TMP_InputField createText;
        [SerializeField] private MemoCreatedButton createdButton;

        private int n = 0;

        private string _currentText;
        
        [field:SerializeField, SerializedDictionary("Human","TitleName")] public SerializedDictionary<Human, string> memoDict
        {
            get;
            private set;
        }

        private MemoUI _currentFix;
        
        
        private void Awake()
        {
            if (DataManager.Instance.TryGetValue("Memo", out string savedMemo))
            {
                memo = JsonUtility.FromJson<MemoWrapper>(savedMemo);
                n = 0;
                Debug.Log(memo.memos.Count);
                memo.memos.ForEach(m => m.num = n++);
                foreach (var m in memo.memos)
                {
                    CreateMemo(m.human, m.text , m.num);
                }
            }
            createdButton.SetMemoSystem(this);
        }
        
        public void NewCharacter(Human human)
        {
            DataManager.Instance.SaveData(human + "IsFind", true);
            createdButton.SetMemoSystem(this);
        }
        
        private void CreateMemo(Human human, string text, int mNum)
        {
            GameObject go = Instantiate(prefab, tParent);
            MemoUI memoUI = go.GetComponent<MemoUI>();
            memoUI.SetMemo(human, text, this);
            memoUI.Num = mNum;
            memoUI.clicked.AddListener(StartFix);
        }

        public void StartCreate()
        {
            createPanel.SetActive(true);
            createText.text = string.Empty;
            createdButton.SaveOpen(false);
            createdButton.DelOpen(false);
            createdButton.ChangeCurrentGuestToFirst();
            _currentText = "";
            _currentFix = null;
        }

        private void StartFix(MemoUI currentMemo)
        {
            createPanel.SetActive(true);
            createdButton.SaveOpen(!string.IsNullOrWhiteSpace(currentMemo.Text));
            createdButton.DelOpen(true);
            createText.text = currentMemo.Text;
            _currentText = currentMemo.Text;
            createdButton.ChangeCurrentGuest(currentMemo.Human);
            _currentFix = currentMemo;
        }

        public void EndEdit(string text)
        {
            createdButton.SaveOpen(!string.IsNullOrWhiteSpace(text));
            _currentText = text;
        }

        public void EndCreate()
        {
            if (_currentFix)
            {
                _currentFix.SetMemo(createdButton.CurrentGuest, _currentText, this);
                int num = memo.memos.FindIndex(m => m.num == _currentFix.Num);
                Memo me = memo.memos[num];
                me.human = createdButton.CurrentGuest;
                me.text = _currentText;
                memo.memos[num] = me;
                createText.text = string.Empty;
                createPanel.SetActive(false);
                _currentFix = null;
            }
            else
            {
                memo.memos.Add(new Memo(){human = createdButton.CurrentGuest, text = _currentText, num = n});
                CreateMemo(createdButton.CurrentGuest, _currentText, n++);
                createText.text = string.Empty;
                createPanel.SetActive(false);
            }
            
            DataManager.Instance.SaveData("Memo", JsonUtility.ToJson(memo));
            
            _currentText = "";
        }

        public void Del()
        {
            if (_currentFix)
            {
                int num = memo.memos.FindIndex(m => m.num == _currentFix.Num);
                memo.memos.RemoveAt(num);
                Destroy(_currentFix.gameObject);
                createText.text = string.Empty;
                createPanel.SetActive(false);
                _currentFix = null;
            }
        }

        public void CancelCreate()
        {
            createText.text = string.Empty;
            createPanel.SetActive(false);
            _currentText = "";
        }
    }
}