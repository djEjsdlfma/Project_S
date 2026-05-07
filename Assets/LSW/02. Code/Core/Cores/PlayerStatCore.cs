using System;
using System.Collections.Generic;
using UnityEngine;

namespace LSW._02._Code.Core.Cores
{
    public class PlayerStatCore : MonoBehaviour, ICore
    {
        [SerializeField] private int maxDay;

        public int CurrentDay { get; private set; } = 1;
        public Dictionary<Guest, GuestData> GuestsData { get; private set; } = new Dictionary<Guest, GuestData>();
        
        public event Action<int> OnDayChanged;

        private DialogueDataCore _dialogueDataCore;
        
        public void Initialize(CoreHandler coreHandler)
        {
            _dialogueDataCore = CoreHandler.Instance.GetCore<DialogueDataCore>();
            if(_dialogueDataCore == null)
                return;
            
            foreach (Guest guestName in Enum.GetValues(typeof(Guest)))
            {
                if (!_dialogueDataCore.GetAllDialogueEntry(guestName.ToString(), out _))
                    continue;

                GuestData newGuestData = new GuestData
                {
                    CurrentSincerityAmount = 0
                };

                GuestsData.TryAdd(guestName, newGuestData);

                Debug.Log($"Added Guest : {guestName}");
            }
        }

        public void LoadScene(SceneType sceneType) { }

        [ContextMenu("Increase Day Debug")]
        public void IncreaseDayDebug() => IncreaseDay();
        
        public void IncreaseDay(int increaseAmount = 1)
        {
            CurrentDay = Mathf.Clamp(CurrentDay + increaseAmount, 1, maxDay);
            OnDayChanged?.Invoke(CurrentDay);
        }

        public void ChangeSincerityAmount(string guestName, int amount)
        {
            if(!Enum.TryParse(guestName, out Guest guest))
                return;

            GuestData guestData = GuestsData[guest];
            int finalAmount = Mathf.Clamp(guestData.CurrentSincerityAmount + amount, 0, 100);
            guestData.CurrentSincerityAmount = finalAmount;
            GuestsData[guest] = guestData;
            
            Debug.Log($"{guestName}: {guestData.CurrentSincerityAmount}");
        }

        public void Reset() { }
        
    }

    public struct GuestData
    {
        public int CurrentSincerityAmount { get; set; }
    }
    
    public enum Guest
    {
        Sheet1 = -1,        // 테스트용임
        None = 0,
    }
}