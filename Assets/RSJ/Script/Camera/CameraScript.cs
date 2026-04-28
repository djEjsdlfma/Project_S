using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using MoonLib.ScriptFinder_Pro.RunTime.DevLogs;

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
        HandleTestInput();
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
    /// F 키 입력: 테스트용으로 현재 위치 검사
    /// </summary>
    private void HandleTestInput()
    {
        if (!Keyboard.current.fKey.wasPressedThisFrame)
            return;

        StopCopy();
        _img.color = new Color(1, 1, 1, 1);
        CheckObj();
        _img.DOFade(0f, 0.2f);
        Debug.Log("찰칵");
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
            if (item == null) continue;

            Vector2 itemMin = item.bounds.min;
            Vector2 itemMax = item.bounds.max;

            // 체크 박스와 겹치지 않으면 제외
            if (itemMax.x < checkMin.x || itemMin.x > checkMax.x ||
                itemMax.y < checkMin.y || itemMin.y > checkMax.y)
                continue;

            Vector2 clippedMin = new Vector2(
                Mathf.Max(itemMin.x, checkMin.x),
                Mathf.Max(itemMin.y, checkMin.y)
            );

            Vector2 clippedMax = new Vector2(
                Mathf.Min(itemMax.x, checkMax.x),
                Mathf.Min(itemMax.y, checkMax.y)
            );

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
                // 일부만 들어오면 스프라이트를 잘라서 복사
                Sprite clippedSprite = ClipSprite(sr, clippedMin, clippedMax);
                if (clippedSprite == null)
                    continue;

                obj = Instantiate(item.gameObject);
                SpriteRenderer newSr = obj.GetComponent<SpriteRenderer>();
                newSr.sprite = clippedSprite;
                newSr.sortingOrder = sr.sortingOrder + 1;

                realCenter = (clippedMin + clippedMax) * 0.5f;
                obj.transform.position = realCenter;

                // 콜라이더도 잘린 영역에 맞게 재생성
                ReplaceClippedCollider(item.gameObject, obj, clippedMin, clippedMax);
            }

            // 복사본은 처음엔 충돌 비활성화
            if (obj.TryGetComponent(out Collider2D col))
            {
                col.enabled = false;
            }

            // 복사본 위치는 카메라/마우스 위치를 따라가게 저장
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
    /// 잘린 스프라이트 복사본에 맞게 콜라이더를 새로 생성한다.
    /// </summary>
    private void ReplaceClippedCollider(GameObject originalItem, GameObject copiedObj, Vector2 checkMin, Vector2 checkMax)
    {
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

        PolygonCollider2D polyCol = null;
        int pathIndex = 0;

        foreach (var path in originalPaths)
        {
            // 카메라 영역으로 다각형 자르기
            List<Vector2> clippedWorldPath = ClipPolygonAgainstAABB(path, checkMin, checkMax);

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
    /// 원본 스프라이트를 월드 좌표 기준으로 잘라 새 스프라이트를 만든다.
    /// </summary>
    private Sprite ClipSprite(SpriteRenderer sr, Vector2 worldMin, Vector2 worldMax)
    {
        Sprite original = sr.sprite;
        Texture2D originalTex = original.texture;

        Texture2D readableTex = GetReadableTexture(originalTex);
        float ppu = original.pixelsPerUnit;

        Vector2 objWorldMin = sr.bounds.min;
        Vector2 objWorldSize = sr.bounds.size;
        Rect spriteRect = original.textureRect;

        // 월드 좌표를 스프라이트 내부 비율(0~1)로 변환
        float u0 = (worldMin.x - objWorldMin.x) / objWorldSize.x;
        float v0 = (worldMin.y - objWorldMin.y) / objWorldSize.y;
        float u1 = (worldMax.x - objWorldMin.x) / objWorldSize.x;
        float v1 = (worldMax.y - objWorldMin.y) / objWorldSize.y;

        int px = Mathf.RoundToInt(spriteRect.x + u0 * spriteRect.width);
        int py = Mathf.RoundToInt(spriteRect.y + v0 * spriteRect.height);
        int pw = Mathf.RoundToInt((u1 - u0) * spriteRect.width);
        int ph = Mathf.RoundToInt((v1 - v0) * spriteRect.height);

        if (pw <= 0 || ph <= 0)
            return null;

        px = Mathf.Clamp(px, 0, readableTex.width - 1);
        py = Mathf.Clamp(py, 0, readableTex.height - 1);
        pw = Mathf.Clamp(pw, 1, readableTex.width - px);
        ph = Mathf.Clamp(ph, 1, readableTex.height - py);

        Color[] pixels = readableTex.GetPixels(px, py, pw, ph);
        Texture2D newTex = new Texture2D(pw, ph, TextureFormat.RGBA32, false);

        newTex.filterMode = originalTex.filterMode;
        newTex.wrapMode = TextureWrapMode.Clamp;

        newTex.SetPixels(pixels);
        newTex.Apply();

        return Sprite.Create(
            newTex,
            new Rect(0, 0, pw, ph),
            new Vector2(0.5f, 0.5f),
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

    /// <summary>
    /// checkPos 위치에 있는 오브젝트의 색 정보를 가져온다.
    /// </summary>
    private void CheckObj()
    {
        Collider2D item = Physics2D.OverlapBox(
            checkPos.position,
            myPosition.sizeDelta * (CHECK_BOX_SCALE * (_camera.orthographicSize * 0.2f)),
            0,
            coloredObject
        );

        if (item != null && item.gameObject.TryGetComponent(out ColoredObject colorObj))
        {
            _nowColor = colorObj.color;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (myPosition != null && checkPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(checkPos.position, myPosition.sizeDelta * CHECK_BOX_SCALE);
        }
    }
}