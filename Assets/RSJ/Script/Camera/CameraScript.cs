using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using LSW._02._Code.Environment.InteractableObject;
using Moon._01.Script.Cameras;
using MoonLib.ScriptFinder_Pro.RunTime.DevLogs;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.ListFinder;

/// <summary>
/// 화면에 복사된 오브젝트를 원본 기준 오프셋으로 따라다니게 하는 정보 구조체.
/// </summary>
public struct CopiedObj
{
    public Vector2 camToPos;
    public GameObject copyObj;

    public CopiedObj(Vector2 camToPos, GameObject copyObj)
    {
        this.camToPos = camToPos;
        this.copyObj = copyObj;
    }

    public void ChangeTransform(Vector2 camPos)
    {
        if (copyObj)
        {
            copyObj.transform.position = camPos + camToPos;
        }
    }
}

public class CameraScript : MonoBehaviour
{
    [Header("Target / UI")]
    [SerializeField] private Transform checkPos;          // 복사 판정을 할 기준 위치
    [SerializeField] private LayerMask coloredObject;     // 복사 대상 레이어
    [SerializeField] private Image _img;                  // UI 이미지 페이드 연출용

    [SerializeField] private ScriptListFinderSO camerasFinder;

    private RectTransform myPosition;
    private Vector3 _position;
    private MyColor _nowColor;

    private bool _copying = false;

    // 원본 오브젝트 -> 복사된 오브젝트 정보
    private readonly Dictionary<GameObject, CopiedObj> _copyObjs = new();
    private Camera _camera;

    private const float CHECK_BOX_SCALE = 0.009f;

    private void Awake()
    {
        _camera = Camera.main;
        myPosition = GetComponent<RectTransform>();
        _nowColor = MyColor.None;
    }

    private void Update()
    {
        UpdateMouseFollowerUI();

        Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 복사 중이면, 복사된 오브젝트들을 카메라/마우스 기준으로 계속 이동
        if (_copying)
        {
            foreach (var camObj in _copyObjs)
            {
                camObj.Value.ChangeTransform(worldMousePos);
            }
        }

        HandleCopyInput(worldMousePos);
        HandlePhotoInput();
    }
    
    private void HandlePhotoInput()
    {
        if (!Keyboard.current.fKey.wasPressedThisFrame || !camerasFinder.GetTarget<PhotoStorage>().CanPhoto())
            return;

        StopCopy();
        _img.color = new Color(1, 1, 1, 1);
        CheckObj();
        _img.DOFade(0f, 0.2f);
        DevLog.Log("찰칵");
    }

    private void CheckObj()
    {
        camerasFinder.GetTarget<CameraCapture>().TakePhoto();
    }

    /// <summary>
    /// 마우스를 따라 UI가 움직이도록 갱신.
    /// </summary>
    private void UpdateMouseFollowerUI()
    {
        _position = new Vector3(
            Input.mousePosition.x - (myPosition.sizeDelta.x / 2),
            Input.mousePosition.y - (myPosition.sizeDelta.y / 2),
            0f
        );

        myPosition.anchoredPosition = _position;
    }

    /// <summary>
    /// G 키 입력: 복사 시작 / 복사 확정
    /// </summary>
    private void HandleCopyInput(Vector2 worldMousePos)
    {
        if (!Keyboard.current.gKey.wasPressedThisFrame)
            return;

        if (_copying)
        {
            // 이미 복사 중이면 현재 복사본을 "확정"
            PressObj();
            DevLog.Log("Pressed");
        }
        else
        {
            // 새 복사 시작
            _img.color = new Color(1, 1, 1, 1);
            CopyObj(worldMousePos);
            _img.DOFade(0f, 0.2f);
            DevLog.Log("Copy");
        }
    }
    
    /// <summary>
    /// 복사본을 확정하는 동작.
    /// </summary>
    private void PressObj()
    {
        StopCopy(false);
    }

    /// <summary>
    /// 복사 상태 종료.
    /// b == true  : 복사본 삭제
    /// b == false : 복사본 유지 + 콜라이더만 활성화
    /// </summary>
    private void StopCopy(bool b = true)
    {
        _copying = false;

        if (b)
        {
            // 복사본 전부 제거
            foreach (var camObj in _copyObjs)
            {
                if (camObj.Value.copyObj)
                {
                    Destroy(camObj.Value.copyObj);
                }
            }
        }
        else
        {
            // 복사본은 유지하고, 충돌 판정만 다시 켬
            foreach (var camObj in _copyObjs)
            {
                if (camObj.Value.copyObj && camObj.Value.copyObj.TryGetComponent(out Collider2D col))
                {
                    col.enabled = true;
                    if(camObj.Key.TryGetComponent(out ICopyable copyable))
                        copyable.Paste();
                }
            }
        }

        _copyObjs.Clear();
    }

    /// <summary>
    /// checkPos 주변의 오브젝트를 검사해서, 화면에 복사본을 생성.
    /// </summary>
    private void CopyObj(Vector2 pos)
    {
        Collider2D[] items = Physics2D.OverlapBoxAll(
            checkPos.position,
            myPosition.sizeDelta * (CHECK_BOX_SCALE * (_camera.orthographicSize * 0.2f)),
            0,
            coloredObject
        );

        if (items == null || items.Length == 0)
            return;

        Vector2 checkMin = (Vector2)checkPos.position - myPosition.sizeDelta * (CHECK_BOX_SCALE * (_camera.orthographicSize * 0.2f) * 0.5f);
        Vector2 checkMax = (Vector2)checkPos.position + myPosition.sizeDelta * (CHECK_BOX_SCALE * (_camera.orthographicSize * 0.2f) * 0.5f);

        foreach (var item in items)
        {
            if (item == null)
                continue;
            
            Vector2 itemMin = item.bounds.min;
            Vector2 itemMax = item.bounds.max;
            
            if (itemMax.x < checkMin.x || itemMin.x > checkMax.x ||
                itemMax.y < checkMin.y || itemMin.y > checkMax.y)
                continue;

            SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
            if (sr == null || sr.sprite == null)
                continue;

            bool inBox =
                itemMin.x >= checkMin.x && itemMax.x <= checkMax.x &&
                itemMin.y >= checkMin.y && itemMax.y <= checkMax.y;

            GameObject obj;
            Vector2 realCenter;

            if (inBox)
            {
                // 오브젝트가 완전히 들어오면 그대로 복사
                obj = Instantiate(item.gameObject);
                SpriteRenderer newSr = obj.GetComponent<SpriteRenderer>();
                newSr.sortingOrder = sr.sortingOrder + 1;

                realCenter = obj.transform.position;
            }
            else
            {
                Sprite clippedSprite = ClipSprite(sr, checkMin, checkMax, out float preservedRatio);
                if (clippedSprite == null)
                    continue;

                obj = Instantiate(item.gameObject);
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
            {
                col.enabled = false;
            }
            
            if(obj.TryGetComponent(out ICopyable copyable))
                copyable.Copy();

            if (!_copyObjs.ContainsKey(item.gameObject))
            {
                _copyObjs.Add(item.gameObject, new CopiedObj(realCenter - pos, obj));
            }
        }

        if (_copyObjs.Count == 0)
            return;

        _copying = true;
    }

    /// <summary>
    /// 잘린 스프라이트 복사본에 맞게 콜라이더를 새로 생성하고, 남은 면적 비율을 반환한다.
    /// </summary>
    private void ReplaceClippedCollider(GameObject originalItem, GameObject copiedObj, Vector2 checkMin, Vector2 checkMax)
    {
        float preservedRatio = 0f;

        // 복사본에 붙은 기존 콜라이더 제거
        foreach (var col in copiedObj.GetComponents<Collider2D>())
        {
            Destroy(col);
        }

        Collider2D originalCol = originalItem.GetComponent<Collider2D>();
        if (originalCol == null)
            return;

        // 원본 콜라이더를 월드 좌표 기준 다각형으로 변환
        List<List<Vector2>> originalPaths = GetWorldPathsFromCollider(originalCol);

        float originalTotalArea = 0f;
        float clippedTotalArea = 0f;

        PolygonCollider2D polyCol = null;
        int pathIndex = 0;

        foreach (var path in originalPaths)
        {
            // 1. 원본 다각형의 넓이 누적
            originalTotalArea += CalculatePolygonArea(path);

            // 카메라 영역으로 다각형 자르기
            List<Vector2> clippedWorldPath = ClipPolygonAgainstAABB(path, checkMin, checkMax);

            // 2. 잘려나간 후 남은 다각형의 넓이 누적
            clippedTotalArea += CalculatePolygonArea(clippedWorldPath);

            if (clippedWorldPath.Count <= 2)
                continue;

            if (polyCol == null)
                polyCol = copiedObj.AddComponent<PolygonCollider2D>();

            polyCol.pathCount = pathIndex + 1;

            // 월드 좌표 -> 복사본 로컬 좌표 변환
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

        // 3. 최종 비율 계산 (0.0 ~ 1.0)
        if (originalTotalArea > 0f)
        {
            preservedRatio = clippedTotalArea / originalTotalArea;
        }
        
        CamObject camObj = copiedObj.GetComponent<CamObject>();
        
        if (camObj)
        {
            camObj.Ratio = preservedRatio;
        }
    }

    /// <summary>
    /// 사각형 영역(AABB)에 맞춰 다각형을 잘라낸다.
    /// </summary>
    private List<Vector2> ClipPolygonAgainstAABB(List<Vector2> poly, Vector2 min, Vector2 max)
    {
        poly = ClipEdge(poly, 0, min.x); // Left
        poly = ClipEdge(poly, 1, max.x); // Right
        poly = ClipEdge(poly, 2, min.y); // Bottom
        poly = ClipEdge(poly, 3, max.y); // Top
        return poly;
    }

    /// <summary>
    /// 한 변 기준으로 폴리곤을 클리핑한다.
    /// edge:
    /// 0 = Left, 1 = Right, 2 = Bottom, 3 = Top
    /// </summary>
    private List<Vector2> ClipEdge(List<Vector2> poly, int edge, float value)
    {
        if (poly.Count == 0)
            return poly;

        List<Vector2> clipped = new List<Vector2>();

        Vector2 prev = poly[^1];
        bool prevInside = IsInside(prev, edge, value);

        foreach (Vector2 curr in poly)
        {
            bool currInside = IsInside(curr, edge, value);

            // 선을 넘으면 교차점 추가
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

    /// <summary>
    /// 현재 점이 클리핑 기준 안쪽인지 검사한다.
    /// </summary>
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

    /// <summary>
    /// 두 점 사이에서 클리핑 선과 만나는 교차점을 구한다.
    /// </summary>
    private Vector2 GetIntersection(Vector2 p1, Vector2 p2, int edge, float value)
    {
        if (edge == 0 || edge == 1)
        {
            // 수직선과의 교차
            float t = (value - p1.x) / (p2.x - p1.x);
            return new Vector2(value, p1.y + t * (p2.y - p1.y));
        }
        else
        {
            // 수평선과의 교차
            float t = (value - p1.y) / (p2.y - p1.y);
            return new Vector2(p1.x + t * (p2.x - p1.x), value);
        }
    }

    /// <summary>
    /// 다양한 Collider2D를 월드 좌표 다각형으로 변환한다.
    /// </summary>
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

    /// <summary>
    /// 캡슐 콜라이더를 근사 다각형으로 변환한다.
    /// </summary>
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
    
    /// <summary>
    /// 신발끈 공식을 사용하여 다각형의 기하학적 넓이를 계산합니다.
    /// </summary>
    private float CalculatePolygonArea(List<Vector2> points)
    {
        // 점이 3개 미만이면 선이나 점이므로 넓이는 0
        if (points.Count < 3) 
            return 0f;

        float area = 0f;
        for (int i = 0; i < points.Count; i++)
        {
            // 다음 인덱스 (마지막 점의 다음은 첫 번째 점)
            int j = (i + 1) % points.Count; 
        
            area += points[i].x * points[j].y;
            area -= points[j].x * points[i].y;
        }

        return Mathf.Abs(area) * 0.5f;
    }

    /// <summary>
    /// 원본 스프라이트를 월드 좌표 기준으로 잘라 새 스프라이트를 만든다.
    /// </summary>
    /// <summary>
    /// 원본 스프라이트를 월드 좌표 카메라 박스(worldMin, worldMax) 기준으로 잘라 새 스프라이트를 만듭니다. (회전/스케일 지원)
    /// </summary>
    private Sprite ClipSprite(SpriteRenderer sr, Vector2 worldMin, Vector2 worldMax, out float preservedRatio)
    {
        preservedRatio = 0f; // 초기화
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

        if (!hasVisiblePixels) 
            return null;
        
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
    
    /// <summary>
    /// 비읽기 텍스처를 읽을 수 있는 Texture2D로 복제한다.
    /// </summary>
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
    
    private void OnDrawGizmosSelected()
    {
        if (myPosition != null && checkPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(checkPos.position, myPosition.sizeDelta * (CHECK_BOX_SCALE * (_camera.orthographicSize * 0.2f)));
        }
    }
}