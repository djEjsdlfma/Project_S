using UnityEngine;
using UnityEngine.Rendering;

namespace Moon._01.Script.Triggers
{
    public class TriggerToVolume : MonoBehaviour, ITriggerable
    {
        private static readonly int Round = Shader.PropertyToID("_Radius");
        [SerializeField] private Volume effectVolume;
        [SerializeField] private Material material;
        
        private MainTrigger _mainTrigger;

        public void Init(MainTrigger mainTrigger)
        {
            _mainTrigger = mainTrigger;
            
            if (effectVolume != null)
            {
                effectVolume.weight = 0f;
                effectVolume.gameObject.SetActive(false);
            }
            
            if(material != null)
            {
                material.SetFloat(Round, 1.25f);
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

                float metRatio = 1.25f - ratio * 1.25f;
                
                material.SetFloat(Round, metRatio);
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