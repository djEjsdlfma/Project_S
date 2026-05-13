using UnityEngine;

namespace Moon._01.Script.Cameras
{
    public class CamObject : MonoBehaviour
    {
        [field: SerializeField] public string Name { get; private set; } = "CamObject";
        
        [SerializeField] private float ratio = 1.0f;
        public float Ratio { get => ratio; set => ratio = value; }
    }
}