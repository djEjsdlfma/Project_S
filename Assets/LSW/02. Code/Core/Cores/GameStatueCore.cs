using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LSW._02._Code.Core.Cores
{
    public class GameStatueCore : MonoBehaviour, ICore
    {
        [SerializeField] private int maxDay;

        public int CurrentDay { get; private set; } = 1;
        public Dictionary<Guest, GuestData> GuestsData { get; private set; } = new Dictionary<Guest, GuestData>();

        private DialogueDataCore _dialogueDataCore;

        public event Action OnDayChanged;
        
        public void Initialize(CoreHandler coreHandler)
        {
            _dialogueDataCore = CoreHandler.Instance.GetCore<DialogueDataCore>();
            if(_dialogueDataCore == null)
                return;
            
            foreach (Guest guest in Enum.GetValues(typeof(Guest)))
            {
                if (guest == Guest.None) continue;

                // Enum 이름이 아닌 실제 시트가 매핑되어 있는지 확인
                if (!_dialogueDataCore.GetSheetNameByGuest(guest, out _))
                    continue;

                GuestData newGuestData = new GuestData
                {
                    CurrentSincerityAmount = 0
                };

                GuestsData.TryAdd(guest, newGuestData);

                Debug.Log($"Added Guest : {guest}");
            }
        }

        public void LoadScene(SceneType sceneType) { }

        [ContextMenu("Increase Day Debug")]
        public void IncreaseDayDebug() => IncreaseDay();
        
        public void IncreaseDay(int increaseAmount = 1)
        {
            CurrentDay = Mathf.Clamp(CurrentDay + increaseAmount, 1, maxDay);
            OnDayChanged?.Invoke();
            //SceneManager.LoadScene((int)SceneType.MainTabletScene);
        }

        public void ChangeSincerityAmount(string guestName, int amount)
        {
            if(!Enum.TryParse(guestName, out Guest guest))
                return;

            if (!GuestsData.TryGetValue(guest, out GuestData guestData))
                return;

            int realAmount = guestData.CurrentSincerityAmount + amount;
            int finalAmount = Mathf.Clamp(realAmount, 0, 100);
            guestData.CurrentSincerityAmount = finalAmount;
            guestData.RealCurrentSincerityAmount = realAmount;
            
            GuestsData[guest] = guestData;
            
            Debug.Log($"{guestName}: {guestData.CurrentSincerityAmount}");
        }

        public void Reset() { }
        
    }

    public struct GuestData
    {
        public int CurrentSincerityAmount { get; set; }     // 현재 호감도 (0 ~ 100)
        public int RealCurrentSincerityAmount { get; set; }     // 진짜 호감도 (0 ~ )(혹시 제한없는 게 필요한 경우)
    }
    
    public enum Guest
    {
        Sheet1 = -2,        // 테스트용임
        Sheet2 = -1,        // 테스트용2임
        None = 0,
        JaeYoonLee,     // 이재윤
        DaEunJung,        // 정다은
        YulPark,        // 박율
        SeoAhYoon,      // 윤서아
        MyeongJinChoi   // 최명진
    }
}