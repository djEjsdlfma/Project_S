using MoonLib.ScriptFinder_Pro.RunTime.Finder.OneFinder;
using UnityEngine;

namespace Moon._01.Script.Triggers
{
    public class TriggerToLarge : MonoBehaviour , ITriggerable
    {
        [SerializeField] private float maxSize = 10f;
        [SerializeField] private float minSize = 1;
        [SerializeField] private float distanceToSizeRate = 1;

        private MainTrigger _mainTrigger;
        
        public void Init(MainTrigger mainTrigger)
        {
            _mainTrigger = mainTrigger;
        }

        private void FixedUpdate()
        {
            if(!_mainTrigger) return;
            if(_mainTrigger.IsActive)
            {
                float xScale = Mathf.Min(Mathf.Max(_mainTrigger.Distance * distanceToSizeRate, minSize), maxSize);
                Vector3 scale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);
                transform.localScale = scale;
            }
            else
            {
                transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);
            }
        }
    }
}
