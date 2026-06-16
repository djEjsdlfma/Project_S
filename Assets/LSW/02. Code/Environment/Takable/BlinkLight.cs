using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace LSW._02._Code.Environment.Takable
{
    public class BlinkLight : MonoBehaviour, ITakable
    {
        [Header("Light Components")]
        [SerializeField] private Light2D lightComponent;

        [Header("Blink Timings")]
        [SerializeField] private float minOnTime = 0.05f;
        [SerializeField] private float maxOnTime = 0.4f;
        [SerializeField] private float minOffTime = 0.05f;
        [SerializeField] private float maxOffTime = 0.2f;
        
        [Header("Resume Timing")]
        [SerializeField] private float resumeDelay = 3.0f;
        
        private bool _wasTaken = false;
        private Coroutine _blinkCoroutine;
        private Coroutine _resumeCoroutine;

        [SerializeField] private UnityEvent onTake;
        
        private void Start()
        {
            if (lightComponent == null)
                lightComponent = GetComponentInChildren<Light2D>();
            
            if (lightComponent != null)
                _blinkCoroutine = StartCoroutine(BlinkRoutine());
        }

        private IEnumerator BlinkRoutine()
        {
            while (!_wasTaken)
            {
                float waitTime = lightComponent.enabled 
                    ? Random.Range(minOnTime, maxOnTime) 
                    : Random.Range(minOffTime, maxOffTime);

                yield return new WaitForSeconds(waitTime);
                
                lightComponent.enabled = !lightComponent.enabled;
            }
        }

        public void Take()
        {
            if(_wasTaken)
                return;
            
            onTake.Invoke();
            _wasTaken = true;
            
            if (_blinkCoroutine != null)
            {
                StopCoroutine(_blinkCoroutine);
                _blinkCoroutine = null;
            }
            
            if (_resumeCoroutine != null)
            {
                StopCoroutine(_resumeCoroutine);
            }
            
            _resumeCoroutine = StartCoroutine(ResumeBlinkRoutine());
        }

        private IEnumerator ResumeBlinkRoutine()
        {
            yield return new WaitForSeconds(resumeDelay);

            _wasTaken = false;
            _blinkCoroutine = StartCoroutine(BlinkRoutine());
        }

        public bool IsDisableCapture()
        {
            return false;
        }

        public bool CanBeTaken()
        {
            return !_wasTaken;
        }
    }
}