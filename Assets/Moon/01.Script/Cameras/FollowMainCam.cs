using System;
using UnityEngine;

namespace Moon._01.Script.Cameras
{
    public class FollowMainCam : MonoBehaviour
    {

        private Camera _camera;
        
        private void Awake()
        {
            _camera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_camera)
            {
                Vector3 newPosition = new Vector3(_camera.transform.position.x, _camera.transform.position.y, transform.position.z);
                transform.position = newPosition;
            }
        }
    }
}