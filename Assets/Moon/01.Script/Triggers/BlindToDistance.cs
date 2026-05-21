using Moon._01.Script.Cameras;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.ListFinder;
using UnityEngine;

namespace Moon._01.Script.Triggers
{
    public enum TriggerState
    {
        Near,
        WaitingForFarAction,
        Far,
        WaitingForNearAction
    }

    public class BlindToDistance : MonoBehaviour, ITriggerable
    {
        private MainTrigger _mainTrigger;

        [SerializeField] private float targetDistance = 10f;

        [SerializeField] private float farActionDelay = 3f;
        [SerializeField] private float nearActionDelay = 2f;

        [SerializeField] private ScriptListFinderSO cameraFinder;
        
        
        private TriggerState _currentState = TriggerState.Near;

        private float _timer = 0f;
        private SetCamBlur _setCamBlur;

        private void Awake()
        {
            _setCamBlur = cameraFinder.GetTarget<SetCamBlur>();
        }

        public void Init(MainTrigger mainTrigger)
        {
            _mainTrigger = mainTrigger;
        }

        private void Update()
        {
            if (_mainTrigger == null) return;

            float currentDistance = _mainTrigger.Distance;

            switch (_currentState)
            {
                case TriggerState.Near:
                    if (currentDistance >= targetDistance)
                    {
                        _currentState = TriggerState.WaitingForFarAction;
                        _timer = 0f;
                    }
                    break;

                case TriggerState.WaitingForFarAction:
                    if (currentDistance < targetDistance)
                    {
                        _currentState = TriggerState.Near;
                        break;
                    }

                    _timer += Time.deltaTime;
                    if (_timer >= farActionDelay)
                    {
                        OnFarActionTriggered();
                        _currentState = TriggerState.Far;
                    }
                    break;

                case TriggerState.Far:
                    if (currentDistance < targetDistance)
                    {
                        _currentState = TriggerState.WaitingForNearAction;
                        _timer = 0f;
                    }
                    break;

                case TriggerState.WaitingForNearAction:
                    if (currentDistance >= targetDistance)
                    {
                        _currentState = TriggerState.Far;
                        break;
                    }

                    _timer += Time.deltaTime;
                    if (_timer >= nearActionDelay)
                    {
                        OnNearActionTriggered();
                        _currentState = TriggerState.Near;
                    }
                    break;
            }
        }
        private void OnFarActionTriggered()
        {
            _setCamBlur.ActiveBlur(true);
        }
        
        private void OnNearActionTriggered()
        {
            _setCamBlur.ActiveBlur(false);
        }
    }
}