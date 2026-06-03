using System;
using Moon._01.Script.Datas;
using Moon._01.Script.Memos;
using UnityEngine;

namespace Moon._97.Test.Script
{
    [DefaultExecutionOrder(-10)]
    public class TestSaveAndLoad : MonoBehaviour
    {
        [SerializeField] private int slot;
        [SerializeField] private Human h;
        [SerializeField] private MemoSystem memo;

        private void Awake()
        {
            DataManager.Instance.LoadSlot(slot);
        }

        [ContextMenu("Save")]
        public void Save()
        {
            DataManager.Instance.SlotSave(slot);
        }
        
        [ContextMenu("Find")]
        public void Find()
        {
            memo.NewCharacter(h);
        }
    }
}