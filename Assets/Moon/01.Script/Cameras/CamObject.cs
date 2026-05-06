using UnityEngine;

namespace Moon._01.Script.Cameras
{
    public class CamObject : MonoBehaviour
    {
        [field: SerializeField] public string Name { get; private set; } = "CamObject";
        public float Ratio { get; set; } = 1.0f;
    }
}