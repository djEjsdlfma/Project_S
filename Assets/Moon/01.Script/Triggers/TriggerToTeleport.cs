using MoonLib.ScriptFinder_Pro.RunTime.Finder.OneFinder;
using UnityEngine;

namespace Moon._01.Script.Triggers
{
    public class TriggerToTeleport : MonoBehaviour , ITriggerable
    {
        [SerializeField] private Transform avoidTeleportPos;
        [SerializeField] private Transform goEndTeleportPos;
        [SerializeField] private ScriptFinderSO playerFinder;

        private MainTrigger _mainTrigger;

        public void Init(MainTrigger mainTrigger)
        {
            _mainTrigger = mainTrigger;
            
            if (_mainTrigger)
            {
                _mainTrigger.OnEndToActive += ActionToGoEnd;
                _mainTrigger.OnAvoidToActive += ActionToAvoid;
            }
        }

        private void ActionToGoEnd()
        {
            if (!_mainTrigger) return;
            if (_mainTrigger.IsActive)
            {
                playerFinder.GetTransform().position = goEndTeleportPos.position;
            }
        }
        
        private void ActionToAvoid()
        {
            if (!_mainTrigger) return;
            if (!_mainTrigger.IsActive)
            {
                playerFinder.GetTransform().position = avoidTeleportPos.position;
            }
        }
    }
}