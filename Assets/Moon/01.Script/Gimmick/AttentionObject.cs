using LSW._02._Code.System___Manager;
using Moon._01.Script.Cameras;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.ListFinder;
using Moon._01.Script.Mouses;
using UnityEngine;

namespace Moon._01.Script.Gimmick
{
    public class AttentionObject : MonoBehaviour
    {
        [SerializeField] private ScriptListFinderSO camerasFinder;
        [SerializeField] private float worldMoveSpeed = 5f;
        [SerializeField] private float mouseSpeedPer = 0.25f;
        [SerializeField] private float stopDistance = 0.05f;
        [SerializeField] private float destroyTime = 2.5f;
        [SerializeField] private float attentionDistance = 7.5f;
        
        private float _timer = 0f;
        
        private Camera _mainCam;

        private MouseManager _mouse;
        private bool _isMouseNotNull;
        
        private bool _isCatched =false;

        private void Start()
        {
            _isMouseNotNull = _mouse != null;
        }

        private void Awake()
        {
            _mainCam = Camera.main;
            _mouse = SystemManager.Instance.GetSystemManager<MouseManager>();
        }
        
        private void OnDestroy()
        {
            if (_isMouseNotNull)
            {
                _mouse.SpeedMultiplier = 1f;
            }
        }

        private void Update()
        {
            if (_mainCam == null || _mouse == null) return;
            
            _timer += Time.deltaTime;
            
            Vector2 currentExactPos = _mouse.ExactScreenPos;

            Vector2 distance = transform.position - _mainCam.ScreenToWorldPoint(currentExactPos);
            
            if (distance.sqrMagnitude > attentionDistance * attentionDistance)
            {
                if (_timer >= destroyTime)
                {
                    Destroy(gameObject);
                }
                if (_isMouseNotNull)
                {
                    _mouse.SpeedMultiplier = 1;
                }
                return;
            }

            if (_isCatched)
            {
                if (_timer >= destroyTime)
                {
                    Destroy(gameObject);
                }
                return;
            }
            
            if (_isMouseNotNull)
            {
                _mouse.SpeedMultiplier = mouseSpeedPer;
            }
            
            float targetDepth = _mainCam.WorldToScreenPoint(transform.position).z;
            Vector2 currentMouseWorldPos = _mainCam.ScreenToWorldPoint(new Vector3(currentExactPos.x, currentExactPos.y, targetDepth));
            
            Vector2 nextWorldPos = Vector2.MoveTowards(currentMouseWorldPos, transform.position, worldMoveSpeed * Time.deltaTime);

            Vector3 nextScreenPos3D = _mainCam.WorldToScreenPoint(new Vector3(nextWorldPos.x, nextWorldPos.y, targetDepth));
            Vector2 finalScreenPos = new Vector2(nextScreenPos3D.x, nextScreenPos3D.y);

            _mouse.WarpToScreenPosition(finalScreenPos);

            if ((nextWorldPos - (Vector2)transform.position).sqrMagnitude <= stopDistance * stopDistance)
            {
                _mouse.StopToMove(destroyTime - _timer);
                _isCatched = true;
            }
           
            if (_timer >= destroyTime)
            {
                Destroy(gameObject);
            }
        }
    }
}