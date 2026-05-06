using System;
using UnityEngine;

namespace LSW._02._Code.Core.Cores
{
    public class PlayerStatCore : MonoBehaviour, ICore
    {
        [SerializeField] private int maxDay;
        
        public int CurrentDay { get; private set; } = 2;
        
        public event Action<int> OnDayChanged;
        
        public void Initialize(CoreHandler coreHandler) { }

        public void LoadScene(SceneType sceneType) { }

        [ContextMenu("Increase Day Debug")]
        public void IncreaseDayDebug() => IncreaseDay();
        
        public void IncreaseDay(int increaseAmount = 1)
        {
            CurrentDay = Mathf.Clamp(CurrentDay + increaseAmount, 1, maxDay);
            OnDayChanged?.Invoke(CurrentDay);
        }

        public void Reset() { }
    }
}