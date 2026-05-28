using System.Collections.Generic;
using DG.Tweening;
using LSW._02._Code.Environment.InteractableObject;
using LSW._02._Code.Environment.Takable;
using LSW._02._Code.System___Manager;
using Moon._01.Script.Cameras;
using Moon._01.Script.Mouses;
using MoonLib.ScriptFinder_Pro.RunTime.DevLogs;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.ListFinder;
using UnityEngine;
using UnityEngine.UI;

namespace RSJ.Script.Camera
{
    public enum CameraMode
    {
        Copy,
        Move
    }

    /// <summary>
    /// 화면에 복사된 오브젝트를 원본 기준 오프셋으로 따라다니게 하는 정보 구조체.
    /// </summary>
    public readonly struct InteractTarget
    {
        private readonly Vector2 _camToPos;
        public readonly GameObject TargetObj;
        public readonly float GravityScale;

        public InteractTarget(Vector2 camToPos, GameObject targetObj, float gravityScale)
        {
            _camToPos = camToPos;
            TargetObj = targetObj;
            GravityScale = gravityScale;
        }

        public void ChangeTransform(Vector2 camPos)
        {
            if (TargetObj)
            {
                TargetObj.transform.position = camPos + _camToPos;
            }
        }
    }

    public class CameraScript : MonoBehaviour
    {
        [field:SerializeField]public bool CanCapture { get; private set; } = true;
    
        [Header("Mode Settings")]
        [SerializeField] private CameraMode _currentMode = CameraMode.Copy;

        [Header("Target / UI")]
        [SerializeField] private Transform checkPos;
        [SerializeField] private LayerMask coloredObject;
        [SerializeField] private Image _img;
        [SerializeField] private ScriptListFinderSO camerasFinder;
        [SerializeField] private CameraInputSO input;
    
        [Header("Components")]
        [SerializeField] private CameraMove _cameraMove; // 분리된 Move 로직 참조

        private RectTransform myPosition;
        private Vector3 _position;
        private UnityEngine.Camera _main;
        private UnityEngine.Camera _camera;

        private MouseManager _mouseManager;

        private bool _copying = false;
        private readonly Dictionary<GameObject, InteractTarget> _interactObjs = new();
    
        // 외부 클래스(CameraMove)에서 접근 가능하도록 public 설정
        public const float CHECK_BOX_SCALE = 0.009f;

        private void Awake()
        {
            _camera = UnityEngine.Camera.main;
            _main = UnityEngine.Camera.main;
            myPosition = GetComponent<RectTransform>();
            _mouseManager = SystemManager.Instance.GetSystemManager<MouseManager>();

            input.OnCaptureAction += HandlePhotoInput;
            input.OnCopyAction += HandleCaptureInput;
        }

        private void OnDestroy()
        {
            input.OnCaptureAction -= HandlePhotoInput;
            input.OnCopyAction -= HandleCaptureInput;
        }

        private void Update()
        {
            UpdateMouseFollowerUI();
        }

        private void FixedUpdate()
        {
            // 복사 중일 때만 본인 스크립트에서 오브젝트를 이동
            if (_copying)
            {
                Vector2 worldMousePos = _mouseManager ? _main.ScreenToWorldPoint(_mouseManager.ExactScreenPos) : input.MousePos;
                foreach (var target in _interactObjs)
                {
                    target.Value.ChangeTransform(worldMousePos);
                }
            }
        }

        public void ChangeMode(CameraMode newMode)
        {
            if (_currentMode == newMode) return;

            if (_copying) StopCopy(true);
            if (_cameraMove != null && _cameraMove.IsMoving) _cameraMove.DropObj();

            _currentMode = newMode;
            DevLog.Log($"Camera Mode Changed to: {_currentMode}");
        }

        private void HandleCaptureInput()
        {
            if (camerasFinder.GetTarget<SetCamBlur>(false) is var blur && blur && blur.BlurActive)
                return;
            HandleActionInput(_mouseManager ? _main.ScreenToWorldPoint(_mouseManager.ExactScreenPos) : input.MousePos);
        }

        private void HandleActionInput(Vector2 worldMousePos)
        {
            switch (_currentMode)
            {
                case CameraMode.Copy:
                    HandleCopyMode(worldMousePos);
                    break;
                case CameraMode.Move:
                    if (_cameraMove != null)
                    {
                        _cameraMove.HandleMoveMode(worldMousePos);
                    }
                    break;
            }
        }

        #region COPY MODE LOGIC
        private void HandleCopyMode(Vector2 worldMousePos)
        {
            if (_copying)
            {
                PressObj();
                DevLog.Log("Copy Confirmed (Pressed)");
            }
            else
            {
                _img.color = new Color(1, 1, 1, 1);
                CopyObj(worldMousePos);
                _img.DOFade(0f, 0.2f);
                DevLog.Log("Copy Start");
            }
        }

        private void PressObj()
        {
            StopCopy(false);
        }

        private void StopCopy(bool cancel = true)
        {
            _copying = false;

            if (cancel)
            {
                foreach (var target in _interactObjs)
                {
                    if (target.Value.TargetObj)
                    {
                        target.Value.TargetObj.SetActive(false);
                        Destroy(target.Value.TargetObj);
                    }
                }
            }
            else
            {
                foreach (var target in _interactObjs)
                {
                    if (target.Value.TargetObj)
                    {
                        if (target.Value.TargetObj.TryGetComponent(out Collider2D col))
                            col.enabled = true;
                    
                        if (target.Value.TargetObj.TryGetComponent(out Rigidbody2D rb))
                            rb.gravityScale = target.Value.GravityScale;
                    
                        if (target.Value.TargetObj.TryGetComponent(out ICopyable copyable))
                            copyable.Paste();
                    }
                }
            }

            _interactObjs.Clear();
        }

        private void CopyObj(Vector2 pos)
        {
            Collider2D[] items = Physics2D.OverlapBoxAll(
                checkPos.position,
                myPosition.sizeDelta * (CHECK_BOX_SCALE * (_camera.orthographicSize * 0.2f)),
                0,
                coloredObject
            );

            if (items == null || items.Length == 0) return;

            Vector2 checkMin = (Vector2)checkPos.position - myPosition.sizeDelta * (CHECK_BOX_SCALE * (_camera.orthographicSize * 0.2f) * 0.5f);
            Vector2 checkMax = (Vector2)checkPos.position + myPosition.sizeDelta * (CHECK_BOX_SCALE * (_camera.orthographicSize * 0.2f) * 0.5f);

            foreach (var item in items)
            {
                if (item == null) continue;
            
                if (item.TryGetComponent(out ITakable _)) continue;
            
                Vector2 itemMin = item.bounds.min;
                Vector2 itemMax = item.bounds.max;
            
                if (itemMax.x < checkMin.x || itemMin.x > checkMax.x ||
                    itemMax.y < checkMin.y || itemMin.y > checkMax.y)
                    continue;

                SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
                if (sr == null || sr.sprite == null) continue;

                bool inBox =
                    itemMin.x >= checkMin.x && itemMax.x <= checkMax.x &&
                    itemMin.y >= checkMin.y && itemMax.y <= checkMax.y;

                GameObject obj;
                Vector2 realCenter;

                if (inBox)
                {
                    obj = Instantiate(item.gameObject, null, true);
                    obj.transform.position = item.transform.position;
                    obj.transform.rotation = item.transform.rotation;
                    obj.transform.localScale = item.transform.lossyScale;

                    SpriteRenderer newSr = obj.GetComponent<SpriteRenderer>();
                    newSr.sortingOrder = sr.sortingOrder + 1;
                
                    realCenter = obj.transform.position;
                }
                else
                {
                    Sprite clippedSprite = ClipSprite(sr, checkMin, checkMax, out float preservedRatio);
                    if (clippedSprite == null) continue;

                    obj = Instantiate(item.gameObject, null, true);
                    obj.transform.position = item.transform.position;
                    obj.transform.rotation = item.transform.rotation;
                    obj.transform.localScale = item.transform.lossyScale;

                    SpriteRenderer newSr = obj.GetComponent<SpriteRenderer>();
                    newSr.sprite = clippedSprite;
                    newSr.sortingOrder = sr.sortingOrder + 1;

                    realCenter = obj.transform.position;
                
                    ReplaceClippedCollider(item.gameObject, obj, checkMin, checkMax);
                
                    if (preservedRatio < 0.5f)
                    {
                        if (obj.TryGetComponent(out Piece piece))
                        {
                            Destroy(piece);
                        }
                    }
                }

                if (obj.TryGetComponent(out Collider2D col))
                    col.enabled = false;
            
                float gravityScale = 0f;
                if (obj.TryGetComponent(out Rigidbody2D rb))
                {
                    gravityScale = rb.gravityScale;
                    rb.gravityScale = 0;
                }
            
                if(obj.TryGetComponent(out ICopyable copyable))
                    copyable.Copy();

                if (!_interactObjs.ContainsKey(item.gameObject))
                {
                    _interactObjs.Add(item.gameObject, new InteractTarget(realCenter - pos, obj, gravityScale));
                }
            }

            if (_interactObjs.Count > 0)
                _copying = true;
        }
        #endregion

        #region PHOTO / UI LOGIC
        private void HandlePhotoInput()
        {
            if (camerasFinder.GetTarget<SetCamBlur>(false) is var blur && blur && blur.BlurActive)
                return;
            if(!CanCapture)
                return;
        
            if (CheckAndTakeObject())
                return; 
        
            if (!camerasFinder.GetTarget<PhotoStorage>().CanPhoto())
                return;
        
            if (_copying) StopCopy(true);
            if (_cameraMove != null && _cameraMove.IsMoving) _cameraMove.DropObj();

            CheckUIInArea();

            _img.color = new Color(1, 1, 1, 1);
            _img.DOFade(0f, 0.2f);
            DevLog.Log("찰칵");
            CheckObj();
        }

        private bool CheckAndTakeObject()
        {
            Collider2D[] items = Physics2D.OverlapBoxAll(
                checkPos.position,
                myPosition.sizeDelta * (CHECK_BOX_SCALE * (_camera.orthographicSize * 0.2f)),
                0,
                coloredObject
            );

            if (items == null || items.Length == 0) return false;

            foreach (var item in items)
            {
                if (item == null) continue;
            
                if (item.TryGetComponent(out ITakable takable))
                {
                    if (takable.IsDisableCapture())
                        CanCapture = false;
                    takable.Take();
                    return true;
                }
            }

            return false;
        }

        private void CheckObj()
        {
            camerasFinder.GetTarget<CameraCapture>().TakePhoto();
        }

        private void CheckUIInArea()
        {
            Vector2 boxSize = myPosition.sizeDelta * (CHECK_BOX_SCALE * (_camera.orthographicSize * 0.2f));
    
            Vector2 worldMin = (Vector2)checkPos.position - (boxSize * 0.5f);
            Vector2 worldMax = (Vector2)checkPos.position + (boxSize * 0.5f);

            Vector2 screenMin = _camera.WorldToScreenPoint(worldMin);
            Vector2 screenMax = _camera.WorldToScreenPoint(worldMax);

            Rect screenRect = Rect.MinMaxRect(
                Mathf.Min(screenMin.x, screenMax.x), 
                Mathf.Min(screenMin.y, screenMax.y), 
                Mathf.Max(screenMin.x, screenMax.x), 
                Mathf.Max(screenMin.y, screenMax.y)
            );

            camerasFinder.GetTarget<CamDelUI>().CheckUI(screenRect);
        }

        private void UpdateMouseFollowerUI()
        {
            Vector2 mousePos = _mouseManager.ExactScreenPos;
            _position = new Vector3(
                mousePos.x - (myPosition.sizeDelta.x / 2),
                mousePos.y - (myPosition.sizeDelta.y / 2),
                0f
            );

            myPosition.anchoredPosition = _position;
        }
        #endregion

        #region POLYGON / CLIPPING UTILS 
    
        private void ReplaceClippedCollider(GameObject originalItem, GameObject copiedObj, Vector2 checkMin, Vector2 checkMax)
        {
            float preservedRatio = 0f;

            foreach (var col in copiedObj.GetComponents<Collider2D>())
            {
                Destroy(col);
            }

            Collider2D originalCol = originalItem.GetComponent<Collider2D>();
            if (originalCol == null) return;

            List<List<Vector2>> originalPaths = GetWorldPathsFromCollider(originalCol);

            float originalTotalArea = 0f;
            float clippedTotalArea = 0f;

            PolygonCollider2D polyCol = null;
            int pathIndex = 0;

            foreach (var path in originalPaths)
            {
                originalTotalArea += CalculatePolygonArea(path);
                List<Vector2> clippedWorldPath = ClipPolygonAgainstAABB(path, checkMin, checkMax);
                clippedTotalArea += CalculatePolygonArea(clippedWorldPath);

                if (clippedWorldPath.Count <= 2) continue;

                if (polyCol == null)
                    polyCol = copiedObj.AddComponent<PolygonCollider2D>();

                polyCol.pathCount = pathIndex + 1;

                Vector2[] localPath = new Vector2[clippedWorldPath.Count];
                for (int i = 0; i < clippedWorldPath.Count; i++)
                {
                    localPath[i] = copiedObj.transform.InverseTransformPoint(clippedWorldPath[i]);
                }

                polyCol.SetPath(pathIndex, localPath);
                pathIndex++;
            }

            if (polyCol != null)
            {
                polyCol.enabled = false;
            }

            if (originalTotalArea > 0f)
            {
                preservedRatio = clippedTotalArea / originalTotalArea;
            }
        
            CamObject camObj = copiedObj.GetComponent<CamObject>();
            if (camObj)
            {
                camObj.Ratio *= preservedRatio;
            }
        }

        private List<Vector2> ClipPolygonAgainstAABB(List<Vector2> poly, Vector2 min, Vector2 max)
        {
            poly = ClipEdge(poly, 0, min.x); // Left
            poly = ClipEdge(poly, 1, max.x); // Right
            poly = ClipEdge(poly, 2, min.y); // Bottom
            poly = ClipEdge(poly, 3, max.y); // Top
            return poly;
        }

        private List<Vector2> ClipEdge(List<Vector2> poly, int edge, float value)
        {
            if (poly.Count == 0) return poly;

            List<Vector2> clipped = new List<Vector2>();
            Vector2 prev = poly[^1];
            bool prevInside = IsInside(prev, edge, value);

            foreach (Vector2 curr in poly)
            {
                bool currInside = IsInside(curr, edge, value);
                if (currInside != prevInside)
                {
                    clipped.Add(GetIntersection(prev, curr, edge, value));
                }
                if (currInside)
                {
                    clipped.Add(curr);
                }
                prev = curr;
                prevInside = currInside;
            }

            return clipped;
        }

        private bool IsInside(Vector2 p, int edge, float value)
        {
            return edge switch
            {
                0 => p.x >= value, // Left
                1 => p.x <= value, // Right
                2 => p.y >= value, // Bottom
                3 => p.y <= value, // Top
                _ => false
            };
        }

        private Vector2 GetIntersection(Vector2 p1, Vector2 p2, int edge, float value)
        {
            if (edge == 0 || edge == 1)
            {
                float t = (value - p1.x) / (p2.x - p1.x);
                return new Vector2(value, p1.y + t * (p2.y - p1.y));
            }
            else
            {
                float t = (value - p1.y) / (p2.y - p1.y);
                return new Vector2(p1.x + t * (p2.x - p1.x), value);
            }
        }

        private List<List<Vector2>> GetWorldPathsFromCollider(Collider2D col)
        {
            List<List<Vector2>> paths = new List<List<Vector2>>();
            Transform t = col.transform;

            if (col is PolygonCollider2D poly)
            {
                for (int i = 0; i < poly.pathCount; i++)
                {
                    Vector2[] path = poly.GetPath(i);
                    List<Vector2> worldPath = new List<Vector2>();
                    foreach (Vector2 p in path)
                    {
                        worldPath.Add(t.TransformPoint(p + poly.offset));
                    }
                    paths.Add(worldPath);
                }
            }
            else if (col is BoxCollider2D box)
            {
                Vector2 halfSize = box.size * 0.5f;
                Vector2 offset = box.offset;
                List<Vector2> worldPath = new List<Vector2>
                {
                    t.TransformPoint(offset + new Vector2(-halfSize.x, -halfSize.y)),
                    t.TransformPoint(offset + new Vector2( halfSize.x, -halfSize.y)),
                    t.TransformPoint(offset + new Vector2( halfSize.x,  halfSize.y)),
                    t.TransformPoint(offset + new Vector2(-halfSize.x,  halfSize.y))
                };
                paths.Add(worldPath);
            }
            else if (col is CircleCollider2D circle)
            {
                List<Vector2> worldPath = new List<Vector2>();
                int segments = 24;
                for (int i = 0; i < segments; i++)
                {
                    float angle = i * Mathf.PI * 2f / segments;
                    Vector2 localPoint = circle.offset + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * circle.radius;
                    worldPath.Add(t.TransformPoint(localPoint));
                }
                paths.Add(worldPath);
            }
            else if (col is CapsuleCollider2D capsule)
            {
                paths.Add(GenerateCapsuleWorldPoints(capsule));
            }

            return paths;
        }

        private List<Vector2> GenerateCapsuleWorldPoints(CapsuleCollider2D capsule)
        {
            List<Vector2> points = new List<Vector2>();
            Transform t = capsule.transform;

            int segments = 12;
            Vector2 size = capsule.size;
            Vector2 offset = capsule.offset;

            float radius = (capsule.direction == CapsuleDirection2D.Vertical ? size.x : size.y) * 0.5f;
            float height = (capsule.direction == CapsuleDirection2D.Vertical ? size.y : size.x);
            float straightLen = Mathf.Max(0, height - radius * 2);

            if (capsule.direction == CapsuleDirection2D.Vertical)
            {
                Vector2 topCenter = offset + new Vector2(0, straightLen * 0.5f);
                Vector2 botCenter = offset + new Vector2(0, -straightLen * 0.5f);

                for (int i = 0; i <= segments; i++)
                {
                    float angle = Mathf.Lerp(0, Mathf.PI, (float)i / segments);
                    points.Add(t.TransformPoint(topCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius));
                }
                for (int i = 0; i <= segments; i++)
                {
                    float angle = Mathf.Lerp(Mathf.PI, Mathf.PI * 2, (float)i / segments);
                    points.Add(t.TransformPoint(botCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius));
                }
            }
            else
            {
                Vector2 rightCenter = offset + new Vector2(straightLen * 0.5f, 0);
                Vector2 leftCenter = offset + new Vector2(-straightLen * 0.5f, 0);

                for (int i = 0; i <= segments; i++)
                {
                    float angle = Mathf.Lerp(-Mathf.PI / 2, Mathf.PI / 2, (float)i / segments);
                    points.Add(t.TransformPoint(rightCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius));
                }
                for (int i = 0; i <= segments; i++)
                {
                    float angle = Mathf.Lerp(Mathf.PI / 2, Mathf.PI * 1.5f, (float)i / segments);
                    points.Add(t.TransformPoint(leftCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius));
                }
            }

            return points;
        }

        private float CalculatePolygonArea(List<Vector2> points)
        {
            if (points.Count < 3) return 0f;

            float area = 0f;
            for (int i = 0; i < points.Count; i++)
            {
                int j = (i + 1) % points.Count; 
                area += points[i].x * points[j].y;
                area -= points[j].x * points[i].y;
            }

            return Mathf.Abs(area) * 0.5f;
        }

        private Sprite ClipSprite(SpriteRenderer sr, Vector2 worldMin, Vector2 worldMax, out float preservedRatio)
        {
            preservedRatio = 0f;
            Sprite original = sr.sprite;
            if (original == null) return null;

            Texture2D originalTex = original.texture;
            Texture2D readableTex = GetReadableTexture(originalTex);

            Rect rect = original.textureRect;
            int rx = Mathf.FloorToInt(rect.x);
            int ry = Mathf.FloorToInt(rect.y);
            int rw = Mathf.CeilToInt(rect.width);
            int rh = Mathf.CeilToInt(rect.height);
        
            rx = Mathf.Clamp(rx, 0, readableTex.width - 1);
            ry = Mathf.Clamp(ry, 0, readableTex.height - 1);
            rw = Mathf.Clamp(rw, 1, readableTex.width - rx);
            rh = Mathf.Clamp(rh, 1, readableTex.height - ry);

            Color[] pixels = readableTex.GetPixels(rx, ry, rw, rh);
            Destroy(readableTex);

            Transform t = sr.transform;
            float ppu = original.pixelsPerUnit;
            Vector2 pivot = original.pivot;

            int minX = rw, minY = rh, maxX = 0, maxY = 0;
            bool hasVisiblePixels = false;

            Vector2 origin = t.TransformPoint(Vector3.zero);
            Vector2 rightStep = ((Vector2)t.TransformPoint(Vector3.right) - origin) / ppu;
            Vector2 upStep = ((Vector2)t.TransformPoint(Vector3.up) - origin) / ppu;
        
            int totalValidPixels = 0;
            int keptPixels = 0;

            for (int y = 0; y < rh; y++)
            {
                for (int x = 0; x < rw; x++)
                {
                    int index = y * rw + x;
                    if (pixels[index].a <= 0.01f) continue;
                
                    totalValidPixels++;
                
                    float localX = x - pivot.x;
                    float localY = y - pivot.y;
                
                    Vector2 worldPos = origin + rightStep * localX + upStep * localY;
                
                    if (worldPos.x < worldMin.x || worldPos.x > worldMax.x ||
                        worldPos.y < worldMin.y || worldPos.y > worldMax.y)
                    {
                        pixels[index] = Color.clear;
                    }
                    else
                    {
                        keptPixels++;
                        if (x < minX) minX = x;
                        if (x > maxX) maxX = x;
                        if (y < minY) minY = y;
                        if (y > maxY) maxY = y;
                        hasVisiblePixels = true;
                    }
                }
            }

            if (!hasVisiblePixels) return null;
        
            if (totalValidPixels > 0)
            {
                preservedRatio = (float)keptPixels / totalValidPixels;
            }
        
            int newW = maxX - minX + 1;
            int newH = maxY - minY + 1;
            Color[] croppedPixels = new Color[newW * newH];

            for (int y = 0; y < newH; y++)
            {
                for (int x = 0; x < newW; x++)
                {
                    croppedPixels[y * newW + x] = pixels[(minY + y) * rw + (minX + x)];
                }
            }

            Texture2D newTex = new Texture2D(newW, newH, TextureFormat.RGBA32, false);
            newTex.filterMode = originalTex.filterMode;
            newTex.wrapMode = TextureWrapMode.Clamp;
            newTex.SetPixels(croppedPixels);
            newTex.Apply();
        
            Vector2 newPivot = new Vector2(pivot.x - minX, pivot.y - minY);
            Vector2 normalizedPivot = new Vector2(newPivot.x / newW, newPivot.y / newH);

            return Sprite.Create(
                newTex,
                new Rect(0, 0, newW, newH),
                normalizedPivot,
                ppu,
                0,
                SpriteMeshType.FullRect
            );
        }
    
        private Texture2D GetReadableTexture(Texture2D source)
        {
            RenderTexture rt = RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear
            );

            Graphics.Blit(source, rt);

            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;

            Texture2D readable = new Texture2D(source.width, source.height);
            readable.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
            readable.Apply();

            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);

            return readable;
        }

        public void SetCanCapture(bool b)
        {
            CanCapture = b;
        }

        private void OnDrawGizmosSelected()
        {
            if (myPosition != null && checkPos != null)
            {
                Gizmos.color = Color.red;
                if (_camera != null)
                {
                    Gizmos.DrawWireCube(checkPos.position, myPosition.sizeDelta * (CHECK_BOX_SCALE * (_camera.orthographicSize * 0.2f)));
                }
            }
        }
        #endregion
    }
}