using LSW._02._Code.System___Manager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Moon._01.Script.Mouses
{
    public class MouseManager : MonoBehaviour , ISystemManager
    {
        
        [field:SerializeField]public float MouseSensitivity { get; set; } = 1f;
        
        public float SpeedMultiplier { get; set; } = 1f;

        private Camera _mainCam;
        public Vector2 ExactScreenPos { get; private set; }
        
        private Vector2 _lastWarpedPos;
        private bool _isInitialized = false;
        
        private float _timer = 0f;

        private bool _isNoStoped = true;
        
        public void Initialize(SystemManager systemManager)
        {
            _mainCam = Camera.main;
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
        
        public void Reset()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void Update()
        {
            if (Mouse.current == null || _mainCam == null) return;
            
            if (_timer > 0)
            {
                _timer -= Time.deltaTime;
                return;
            }
            _isNoStoped = true;

            Vector2 currentScreenPos = Mouse.current.position.value;

            if (!_isInitialized)
            {
                ExactScreenPos = currentScreenPos;
                _lastWarpedPos = currentScreenPos;
                _isInitialized = true;
            }

            Vector2 realMouseDelta = currentScreenPos - _lastWarpedPos;

            Vector2 nextPos = ExactScreenPos + (realMouseDelta * (MouseSensitivity * SpeedMultiplier));
            WarpToScreenPosition(nextPos);
        }

        private void LateUpdate()
        {
            if (Mouse.current == null || !_isInitialized) return;

            if (!_isNoStoped)
            {
                return;
            }
            
            Mouse.current.WarpCursorPosition(ExactScreenPos);
            _lastWarpedPos = ExactScreenPos;
        }
        
        public void WarpToScreenPosition(Vector2 screenPos)
        {
            if(!_isNoStoped)
                return;
            
            screenPos.x = Mathf.Clamp(screenPos.x, 0f, Screen.width);
            screenPos.y = Mathf.Clamp(screenPos.y, 0f, Screen.height);
            
            ExactScreenPos = screenPos;
        }
        
        public Vector2 GetRealMouseDelta()
        {
            if (Mouse.current == null || !_isInitialized) return Vector2.zero;
            return Mouse.current.position.value - _lastWarpedPos;
        }

        public void StopToMove(float stopTime)
        {
            if (_timer < stopTime)
            {
                _timer = stopTime;
                _isNoStoped = true;
            }
        }
    }
}