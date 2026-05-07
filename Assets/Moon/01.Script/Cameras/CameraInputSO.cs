using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Moon._01.Script.Cameras
{
    [CreateAssetMenu(fileName = "CameraInput", menuName = "SO/Input/Camera", order = 0)]
    public class CameraInputSO : ScriptableObject , Inputs.ICameraActions
    {
        public event Action OnCaptureAction;
        public event Action OnCopyAction;
        public Vector2 MousePos { get; private set; }

        private Inputs _input;
        
        private void OnEnable()
        {
            if (_input == null)
            {
                _input = new Inputs();
                _input.Camera.SetCallbacks(this);
            }
            _input.Camera.Enable();
        }

        private void OnDisable()
        {
            _input.Camera.Disable();
        }
        
        public void OnCapture(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnCaptureAction?.Invoke();
        }

        public void OnCopy(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnCopyAction?.Invoke();
        }

        public void OnMousePos(InputAction.CallbackContext context)
        {
            MousePos = context.ReadValue<Vector2>();
        }
    }
}