using System.Linq;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.OneFinder;
using UnityEngine;
using UnityEngine.Events;

namespace Moon._01.Script.Triggers
{
    public class MainTrigger : MonoBehaviour
    {
        [SerializeField] private Transform triggerPos;
        [SerializeField] private Transform triggerObj;
        [SerializeField] private ScriptFinderSO playerFinder;
        [SerializeField] private bool isRight = true;

        public bool IsActive { get; private set; } = false;

        public float Distance { get; private set; } = 0;
        
        public float TriggerDistance { get; private  set; } = 0;

        public bool WorkActive { get; set; } = true;

        public UnityAction OnAvoidToActive;
        public UnityAction OnEndToActive;
        

        private Transform _player;

        private void Awake()
        {
            _player = playerFinder.GetTransform();
            GetComponentsInChildren<ITriggerable>(true).ToList().ForEach(t => t.Init(this));
        }

        private void FixedUpdate()
        {
            if (!_player) return;
            
            if(!WorkActive) return;

            Distance = Mathf.Abs(_player.position.x - triggerPos.position.x);
            
            TriggerDistance = Mathf.Abs(_player.position.x - triggerObj.position.x);

            bool lastActive = WorkActive && IsActive;
            
            if ((isRight && _player.position.x > triggerPos.position.x) ||
                (!isRight && _player.position.x < triggerPos.position.x))
            {
                IsActive = WorkActive;
            }
            else
            {
                IsActive = false;
            }

            if (!IsActive)
            {
                if (lastActive)
                {
                    OnAvoidToActive?.Invoke();
                }
            }

            if (isRight && _player.position.x <= triggerPos.position.x
                || !isRight && _player.position.x >= triggerPos.position.x)
            {
                TriggerDistance = 0;
            }
            
            if (IsActive)
            {
                if(Mathf.Approximately(TriggerDistance, 0f))
                {
                    OnEndToActive?.Invoke();
                    WorkActive = false;
                }
            }
        }
    }
}