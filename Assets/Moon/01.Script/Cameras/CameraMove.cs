using System.Collections.Generic;
using DG.Tweening;
using LSW._02._Code.Environment.Takable;
using Moon._01.Script.Mouses;
using RSJ.Script.Camera;
using UnityEngine;
using UnityEngine.UI;

namespace Moon._01.Script.Cameras
{
    public class CameraMove : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Transform checkPos;
        [SerializeField] private LayerMask coloredObject;
        [SerializeField] private Image _img;
        [SerializeField] private CameraInputSO input;

        private RectTransform _myPosition;
        private Camera _main;
        private Camera _camera;

        private bool _moving = false;
        public bool IsMoving => _moving;

        public bool IsActive { get; set; } = true;

        private readonly Dictionary<GameObject, InteractTarget> _interactObjs = new();
        
        private MouseManager _mouseManager;

        private void Awake()
        {
            _camera = Camera.main;
            _main = Camera.main;
            _myPosition = GetComponent<RectTransform>();
        }

        private void FixedUpdate()
        {
            if (_moving && IsActive)
            {
                Vector2 worldMousePos = _camera.ScreenToWorldPoint(_mouseManager ? _mouseManager.ExactScreenPos : input.MousePos);
                
                foreach (var target in _interactObjs)
                {
                    target.Value.ChangeTransform(worldMousePos);
                }
            }
        }

        /// <summary>
        /// CameraScript에서 이동 모드(G키 입력 등)일 때 호출
        /// </summary>
        public void HandleMoveMode(Vector2 worldMousePos)
        {
            if (_moving)
            {
                DropObj();
            }
            else
            {
                _img.color = new Color(1, 1, 1, 1);
                SelectMoveObj(worldMousePos);
                _img.DOFade(0f, 0.2f);
            }
        }

        private void SelectMoveObj(Vector2 pos)
        {
            Collider2D[] items = Physics2D.OverlapBoxAll(
                checkPos.position,
                _myPosition.sizeDelta * (CameraScript.CHECK_BOX_SCALE * (_camera.orthographicSize * 0.2f)),
                0,
                coloredObject
            );

            if (items == null || items.Length == 0)
                return;
    
            // ✅ pos를 world 좌표로 변환 (Z값 명시)
            Vector3 screenPosVec3 = new Vector3(pos.x, pos.y, 0f);
            Vector2 worldPos = screenPosVec3;
    
            foreach (var item in items)
            {
                if (item == null) continue;
    
                if (item.TryGetComponent(out ITakable _))
                {
                    continue;
                }
                
                if (item.TryGetComponent(out CamObject cmObj) && !cmObj.CanCopyOrMove) continue;
    
                GameObject obj = item.gameObject;
                Vector2 realCenter = obj.transform.position;

                if (obj.TryGetComponent(out Collider2D col))
                    col.enabled = false;
    
                float gravityScale = 0f;
                if (obj.TryGetComponent(out Rigidbody2D rb))
                {
                    gravityScale = rb.gravityScale;
                    rb.gravityScale = 0;
                    rb.linearVelocity = Vector2.zero;
                }

                if (!_interactObjs.ContainsKey(obj))
                {
                    _interactObjs.Add(obj, new InteractTarget(realCenter - worldPos, obj, gravityScale));
                }
            }

            if (_interactObjs.Count > 0)
                _moving = true;
        }

        public void DropObj()
        {
            _moving = false;

            foreach (var target in _interactObjs)
            {
                if (target.Value.TargetObj)
                {
                    // 충돌 및 물리 다시 활성화
                    if (target.Value.TargetObj.TryGetComponent(out Collider2D col))
                        col.enabled = true;
                
                    if (target.Value.TargetObj.TryGetComponent(out Rigidbody2D rb))
                        rb.gravityScale = target.Value.GravityScale;
                }
            }

            _interactObjs.Clear();
        }
    }
}