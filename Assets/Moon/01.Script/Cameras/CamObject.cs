using UnityEngine;

namespace Moon._01.Script.Cameras
{
    public class CamObject : MonoBehaviour
    {
        [field: SerializeField] public string Name { get; private set; } = "CamObject";
    }
}