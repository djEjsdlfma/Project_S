using UnityEngine;

namespace Moon._01.Script.Cameras
{
    public class CamObject : MonoBehaviour
    {
        [field: SerializeField] public string Name { get; protected set; } = "CamObject";
        [field: SerializeField] public bool CanCopyOrMove { get; protected set; } = true;
        
        [SerializeField] protected float ratio = 1.0f;
        public virtual float Ratio { get => ratio; set => ratio = value; }
    }
}