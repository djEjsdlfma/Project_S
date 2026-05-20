using UnityEngine;

namespace Moon._01.Script.Triggers
{
    public class TriggerToRound : MonoBehaviour , ITriggerable
    {
        private static readonly int Round = Shader.PropertyToID("_Radius");
        
        [SerializeField] private Material material;
        
        private MainTrigger _mainTrigger;

        public void Init(MainTrigger mainTrigger)
        {
            _mainTrigger = mainTrigger;
            
            if(material != null)
            {
                material.SetFloat(Round, 1.25f);
            }
        }
        
        private void OnDestroy()
        {
            if(material != null)
            {
                material.SetFloat(Round, 1.25f);
            }
        }
        
        private void Update()
        {
            if (!_mainTrigger) return;

            if (_mainTrigger.IsActive)
            {
                
                float distFromStart = _mainTrigger.Distance;
                float distFromTarget = _mainTrigger.TriggerDistance;
                
                float totalDistance = distFromStart + distFromTarget;
                
                float ratio = 0f;
                if (totalDistance > 0f)
                {
                    ratio = distFromStart / totalDistance; 
                }

                float metRatio = 1.25f - ratio * 1.25f;
                
                material.SetFloat(Round, metRatio);
            }
            else
            {
                material.SetFloat(Round, 1.25f);
            }
        }
    }
}