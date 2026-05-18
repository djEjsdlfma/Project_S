using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LSW._02._Code.System___Manager.Environment
{
    public class LightSystem : MonoBehaviour, ISystemManager
    {
        [SerializeField] private Light2D[] changeableLights;
        
        public void Initialize(SystemManager systemManager) { }

        public void ChangeLight(bool isOn)
        {
            for (int i = 0; i < changeableLights.Length; i++)
            {
                changeableLights[i].enabled = isOn;
            }
        }
        
        public void Reset() { }
    }
}