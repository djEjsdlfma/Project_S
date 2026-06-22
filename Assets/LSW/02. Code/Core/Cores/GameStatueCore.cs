using System;
using System.Collections.Generic;
using System.Linq;
using LSW._02._Code.System___Manager;
using Moon._01.Script.Datas;
using UnityEngine;

namespace LSW._02._Code.Core.Cores
{
    public class GameStatueCore : MonoBehaviour, ICore
    {
        [field:SerializeField] public int MaxDay { get; private set;} = 15;

        public int CurrentDay { get; private set; } = 1;
        public Dictionary<Guest, GuestData> GuestsData { get; private set; } = new Dictionary<Guest, GuestData>();
        public Queue<LastDialogueData> SavedLastDialogueData { get; private set; } = new Queue<LastDialogueData>();
        
        private DialogueDataCore _dialogueDataCore;

        public event Action OnDayChanged;
        
        private Transition _transition;
        
        private SceneType _currentSceneType = SceneType.None;
        private SceneType _lastSceneType = SceneType.None;

        private BubbleManager _bubbleManager;
        
        public void Initialize(CoreHandler coreHandler)
        {
            _transition = coreHandler.GetCore<Transition>();
            _dialogueDataCore = coreHandler.GetCore<DialogueDataCore>();
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

        public void LoadScene(SceneType sceneType)
        {
            UnsubscribeBubbleManager();

            if (sceneType == SceneType.MainTabletScene && _currentSceneType == SceneType.MainTabletScene)
                SavedLastDialogueData.Clear();
            
            if(sceneType == SceneType.MainTabletScene)
                SubscribeBubbleManager();
    
            _lastSceneType = _currentSceneType;
            _currentSceneType = sceneType;
        }

        private void SubscribeBubbleManager()
        {
            _bubbleManager = SystemManager.Instance.GetSystemManager<BubbleManager>();
            if (_bubbleManager != null)
            {
                _bubbleManager.onSpawnMessage += SaveLastDialogueData;
            }
        }

        private void UnsubscribeBubbleManager()
        {
            if (_bubbleManager != null)
            {
                _bubbleManager.onSpawnMessage -= SaveLastDialogueData;
            }
        }

        private bool IsPlatformerScene(SceneType sceneType)
        {
            return sceneType is SceneType.ChoiMyeongJinScene or SceneType.DaEunJungScene or SceneType.LeeJaeYoonScene or SceneType.SeoAhYoonScene or SceneType.YulParkScene;
        }

        private void SaveLastDialogueData(LastDialogueData data)
        {
            SavedLastDialogueData.Enqueue(data);
        }

        [ContextMenu("Increase Day Debug")]
        public void IncreaseDayDebug() => IncreaseDay();
        
        public void IncreaseDay(int increaseAmount = 1)
        {
            CurrentDay = Mathf.Clamp(CurrentDay + increaseAmount, 1, MaxDay);
            DataManager.Instance.SaveData("Day", CurrentDay);
            OnDayChanged?.Invoke();
            _transition.TransitionScene(SceneType.MainTabletScene, TransitionType.DayChange);
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
        }

        public void Reset()
        {
            GuestsData.Clear();
        }
        
        private void OnDestroy()
        {
            UnsubscribeBubbleManager();
        }
        
    }

    public struct GuestData
    {
        public int CurrentSincerityAmount { get; set; }     // 현재 호감도 (0 ~ 100)
        public int RealCurrentSincerityAmount { get; set; }     // 진짜 호감도 (0 ~ )(혹시 제한없는 게 필요한 경우)
    }
    
    [Serializable]
    public enum Guest
    {
        Sheet1 = -2,        // 테스트용임
        Sheet2 = -1,        // 테스트용2임
        None = 0,
        JaeYoonLee,     // 이재윤
        DaEunJung,        // 정다은
        YulPark,        // 박율
        SeoAhYoon,      // 윤서아
        MyeongJinChoi,   // 최명진
        Tutorial        // 튜토리얼용 캐릭
    }
    
    [Serializable]
    public struct LastDialogueData
    {
        public string key;
        public bool wasChatNpc;
    }
}