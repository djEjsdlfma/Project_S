using MoonLib.ScriptFinder_Pro.RunTime.Finder.OneFinder;
using UnityEngine;
using UnityEngine.Rendering;

namespace LSW._02._Code.Environment.Trap
{
    public class SlowedByDistance : MonoBehaviour
    {
        [SerializeField] private float effectRadius = 10f;
        [SerializeField, Range(0f, 1f)] private float minSpeedMultiplier = 0.2f;
        
        [SerializeField] private Volume noiseVolume;

        [SerializeField] private ScriptFinderSO playerFinder;
        
        public float CurrentSpeedMultiplier { get; private set; } = 1f;
        
        private Player.Player _player;

        private void Start()
        {
            if (_player == null &&  playerFinder != null)
            {
                _player = playerFinder.GetTarget<Player.Player>();
            }
            
            if (noiseVolume != null)
            {
                noiseVolume.weight = 0f;
            }
        }

        private void Update()
        {
            float distance = Vector3.Distance(transform.position, _player.transform.position);

            if (distance <= effectRadius)
            {
                float intensity = 1f - (distance / effectRadius);
                
                CurrentSpeedMultiplier = Mathf.Lerp(1f, minSpeedMultiplier, intensity);
                
                _player.SetTrapSpeedMultiplier(CurrentSpeedMultiplier);
                
                if (noiseVolume != null)
                {
                    noiseVolume.weight = intensity;
                }
            }
            else
            {
                if (!Mathf.Approximately(CurrentSpeedMultiplier, 1f))
                {
                    CurrentSpeedMultiplier = 1f;
                    _player.SetTrapSpeedMultiplier(1f);
                }
                
                if (noiseVolume != null)
                {
                    noiseVolume.weight = 0f;
                }
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawWireSphere(transform.position, effectRadius);
        }
    }
}