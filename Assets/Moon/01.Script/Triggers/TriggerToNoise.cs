using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Moon._01.Script.Triggers
{
    public class TriggerToNoise : MonoBehaviour, ITriggerable
    {
        [SerializeField] private Volume effectVolume;
        
        private MainTrigger _mainTrigger;

        public void Init(MainTrigger mainTrigger)
        {
            _mainTrigger = mainTrigger;
            
            if (effectVolume != null)
            {
                effectVolume.weight = 0f;
                effectVolume.gameObject.SetActive(false);
            }
        }

        

        private void Update()
        {
            if (_mainTrigger == null || effectVolume == null) return;

            if (_mainTrigger.IsActive)
            {
                effectVolume.gameObject.SetActive(true);
                
                float distFromStart = _mainTrigger.Distance;
                float distFromTarget = _mainTrigger.TriggerDistance;
                
                float totalDistance = distFromStart + distFromTarget;
                
                float ratio = 0f;
                if (totalDistance > 0f)
                {
                    ratio = distFromStart / totalDistance; 
                }

                effectVolume.weight = Mathf.Clamp01(ratio);
            }
            else
            {
                if (effectVolume.weight > 0f || effectVolume.gameObject.activeSelf)
                {
                    effectVolume.weight = 0f;
                    effectVolume.gameObject.SetActive(false);
                }
            }
        }
    }
}