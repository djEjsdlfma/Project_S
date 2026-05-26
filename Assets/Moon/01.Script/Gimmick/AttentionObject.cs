using System;
using Moon._01.Script.Cameras;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.ListFinder;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Moon._01.Script.Gimmick
{
    public class AttentionObject : MonoBehaviour
    {
        [SerializeField] private ScriptListFinderSO camerasFinder;
        [SerializeField] private float worldMoveSpeed = 5f;
    
        private Camera _mainCam;
        
        private Vector2 _exactScreenPos; 
        private bool _isInitialized = false;

        private void Awake()
        {
            _mainCam = Camera.main;
            camerasFinder.GetTarget<CameraMove>();
        }

        private void Update()
        {
            if (Mouse.current == null || _mainCam == null) return;

            if (!_isInitialized)
            {
                _exactScreenPos = Mouse.current.position.value;
                _isInitialized = true;
            }

            float targetDepth = _mainCam.WorldToScreenPoint(transform.position).z;
            
            Vector2 currentMouseWorldPos = _mainCam.ScreenToWorldPoint(new Vector3(_exactScreenPos.x, _exactScreenPos.y, targetDepth));

            Vector2 nextWorldPos = Vector2.MoveTowards(currentMouseWorldPos, transform.position, worldMoveSpeed * Time.deltaTime);

            _exactScreenPos = _mainCam.WorldToScreenPoint(new Vector3(nextWorldPos.x, nextWorldPos.y, targetDepth));

            Mouse.current.WarpCursorPosition(_exactScreenPos);

            if (Vector2.Distance(currentMouseWorldPos, transform.position) <= 0.05f)
            {
                Debug.Log("마우스 이동 완료! (World Unit 기준, 완벽한 직선)");
                Destroy(gameObject);
            }
        }
    }
}