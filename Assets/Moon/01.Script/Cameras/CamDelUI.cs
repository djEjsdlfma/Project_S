using System.Collections.Generic;
using Moon._01.Script.Datas;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.OneFinder;
using UnityEngine;

namespace Moon._01.Script.Cameras
{
    public class CamDelUI : MonoBehaviour
    {
        [SerializeField] private GameObject targetPrefab;
        [SerializeField] private RectTransform targetParent;
        [SerializeField] private float minDistanceToPlayerWorldDis = 1f;
        [SerializeField] private float createTime;
        [SerializeField] private int maxUICount = 5;
        [SerializeField] private ScriptFinderSO playerFinder;
        private List<RectTransform> _targetUIList = new List<RectTransform>();

        private float _timer;
        private bool _canCreate;

        private Transform _player;
        private Camera _mainCamera;
        
        private void Awake()
        {
            _player = playerFinder.GetTransform();
            _mainCamera = Camera.main;
        }

        [ContextMenu("Test")]
        public void Test()
        {
            SetCreate(true);
        }
        
        public void SetCreate(bool canCreate)
        {
            _canCreate = canCreate;
        }

        private void Update()
        {
            if (_canCreate)
            {
                if(_targetUIList.Count >= maxUICount) return;
                _timer += Time.deltaTime;
                if (_timer >= createTime)
                {
                    _ = CreateUI();
                    _timer = 0f;
                }
            }
        }

        public bool CreateUI()
        {
            if (_player == null || _mainCamera == null) return false;

            RectTransform newUI = Instantiate(targetPrefab, targetParent).GetComponent<RectTransform>();
            _targetUIList.Add(newUI);

            newUI.localScale = Vector3.one;
            newUI.anchorMin = new Vector2(0.5f, 0.5f);
            newUI.anchorMax = new Vector2(0.5f, 0.5f);
            newUI.pivot = new Vector2(0.5f, 0.5f);

            float paddingX = newUI.rect.width * 0.5f;
            float paddingY = newUI.rect.height * 0.5f;

            bool isPositionFound = false;
            Rect parentRect = targetParent.rect;

            Canvas canvas = targetParent.GetComponentInParent<Canvas>();
            Camera uiCamera = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : _mainCamera;

            for (int i = 0; i < 30; i++)
            {
                float randX = Random.Range(parentRect.xMin + paddingX, parentRect.xMax - paddingX);
                float randY = Random.Range(parentRect.yMin + paddingY, parentRect.yMax - paddingY);
                newUI.localPosition = new Vector3(randX, randY, 0f);

                Vector2 uiScreenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, newUI.position);
                Vector3 uiWorldPos = _mainCamera.ScreenToWorldPoint(new Vector3(uiScreenPos.x, uiScreenPos.y, Mathf.Abs(_mainCamera.transform.position.z - _player.position.z)));
                uiWorldPos.z = _player.position.z; 

                if (Vector3.Distance(uiWorldPos, _player.position) < minDistanceToPlayerWorldDis)
                {
                    continue;
                }

                Vector2 playerScreenPos = _mainCamera.WorldToScreenPoint(_player.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(targetParent, playerScreenPos, uiCamera, out Vector2 playerLocalPos);

                Vector3 directionToPlayer = (Vector3)playerLocalPos - newUI.localPosition;
                directionToPlayer.z = 0;
                
                if (directionToPlayer != Vector3.zero)
                {
                    newUI.up = directionToPlayer.normalized;
                }

                Rect newRect = GetUIRect(newUI);
                bool isOverlapping = false;

                foreach (RectTransform existingUI in _targetUIList)
                {
                    if (existingUI == newUI) continue;

                    Rect existingRect = GetUIRect(existingUI);
                    if (newRect.Overlaps(existingRect))
                    {
                        isOverlapping = true;
                        break;
                    }
                }

                if (!isOverlapping)
                {
                    isPositionFound = true;
                    break;
                }
            }

            if (!isPositionFound)
            {
                _targetUIList.Remove(newUI);
                newUI.gameObject.SetActive(false);
                Destroy(newUI.gameObject);
                
                return false;
            }

            return true;
        }
        
        public void CheckUI(Rect photoScreenRect)
        {
            Canvas canvas = targetParent.GetComponentInParent<Canvas>();
            Camera uiCamera = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : _mainCamera;

            for (int i = _targetUIList.Count - 1; i >= 0; i--)
            {
                RectTransform targetUI = _targetUIList[i];
        
                Rect uiScreenRect = GetUIScreenRect(targetUI, uiCamera);

                if (photoScreenRect.Overlaps(uiScreenRect))
                {
                    _targetUIList.RemoveAt(i);
                    targetUI.gameObject.SetActive(false);
                    Destroy(targetUI.gameObject);
                }
            }
        }

        private Rect GetUIScreenRect(RectTransform rectTransform, Camera uiCamera)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
    
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            for (int i = 0; i < 4; i++)
            {
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[i]);

                if (screenPoint.x < minX) minX = screenPoint.x;
                if (screenPoint.x > maxX) maxX = screenPoint.x;
                if (screenPoint.y < minY) minY = screenPoint.y;
                if (screenPoint.y > maxY) maxY = screenPoint.y;
            }

            return Rect.MinMaxRect(minX, minY, maxX, maxY);
        }
        
        private Rect GetUIRect(RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            
            float minX = corners[0].x;
            float maxX = corners[0].x;
            float minY = corners[0].y;
            float maxY = corners[0].y;

            for (int i = 1; i < 4; i++)
            {
                if (corners[i].x < minX) minX = corners[i].x;
                if (corners[i].x > maxX) maxX = corners[i].x;
                if (corners[i].y < minY) minY = corners[i].y;
                if (corners[i].y > maxY) maxY = corners[i].y;
            }

            return Rect.MinMaxRect(minX, minY, maxX, maxY);
        }
    }
}