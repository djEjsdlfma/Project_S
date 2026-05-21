using System;
using UnityEngine;

namespace Moon._01.Script.Cameras
{
    public class CamFollowTarget : MonoBehaviour
    {
        [SerializeField] private Transform target;

        private Camera _camera;
        
        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (target)
            {
                Vector3 newPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
                transform.position = newPosition;
                if (_camera)
                {
                    _camera.Render();
                }
            }
        }
    }
}