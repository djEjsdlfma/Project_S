using UnityEngine;

namespace Moon._01.Script.Cameras
{
    public class FindCamAndAddToCanvas : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;

        private void Awake()
        {
            if (canvas == null)
            {
                return;
            }

            Camera mainCamera = Camera.main;
             
            if (mainCamera != null)
            {
                canvas.worldCamera = mainCamera;
            }
        }
    }
}