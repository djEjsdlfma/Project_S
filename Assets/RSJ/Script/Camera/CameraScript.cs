using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using MoonLib.ScriptFinder_Pro.RunTime.DevLogs;

public struct CopyedObj
{
    public Vector2 _camToPos;
    public GameObject _copyObj;

    public CopyedObj(Vector2 _camToPos, GameObject _copyObj)
    {
        this._camToPos = _camToPos;
        this._copyObj = _copyObj;
    }

    public void ChangeTransform(Vector2 camPos)
    {
        if (_copyObj)   
        {
            _copyObj.transform.position = camPos + _camToPos;
        }
    }
}

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Transform checkPos;
    [SerializeField] private LayerMask coloredObject;
    [SerializeField] private Image _img;

    private RectTransform myPosition;
    private Vector3 _position;
    private MyColor _nowColor;

    private bool _copying = false;

    private Dictionary<GameObject, CopyedObj> _copyObjs = new();

    private void Awake()
    {
        myPosition = GetComponent<RectTransform>();
        _nowColor = MyColor.None;
    }

    private void Update()
    {
        _position = new Vector3(Input.mousePosition.x - (myPosition.sizeDelta.x / 2),
            Input.mousePosition.y - (myPosition.sizeDelta.y / 2));
        myPosition.anchoredPosition = _position;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (_copying)
        {
            foreach (var camObj in _copyObjs)
            {
                camObj.Value.ChangeTransform(pos);
            }
        }

        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            if (_copying)
            {
                PressObj();
                DevLog.Log("Pressed");
            }
            else
            {
                _img.color = new Color(1, 1, 1, 1);
                CopyObj(pos);
                _img.DOFade(0f, 0.2f);
                DevLog.Log("Copy");
            }
        }

        //Test
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            StopCopy();
            _img.color = new Color(1, 1, 1, 1);
            CheckObj();
            _img.DOFade(0f, 0.2f);
            Debug.Log("찰칵");
        }
    }
    
    private void PressObj()
    {
        StopCopy(false);
    }

    private void StopCopy(bool b = true)
    {
        _copying = false;
        if (b)
        {
            foreach (var camObj in _copyObjs)
            {
                if (camObj.Value._copyObj)
                {
                    Destroy(camObj.Value._copyObj);
                }
            }
        }
        else
        {
            foreach (var camObj in _copyObjs)
            {
                camObj.Value._copyObj.GetComponent<Collider2D>().enabled = true;
            }
        }

        _copyObjs.Clear();
    }

    private void CopyObj(Vector2 pos)
    {
        Collider2D[] items = Physics2D.OverlapBoxAll(checkPos.position, (myPosition.sizeDelta * 0.009f ), 0, coloredObject);

        if (items == null) return;

        Vector2 checkMin = (Vector2)checkPos.position - myPosition.sizeDelta * (0.009f * 0.5f);
        Vector2 checkMax = (Vector2)checkPos.position + myPosition.sizeDelta * (0.009f * 0.5f);

        foreach (var item in items)
        {
            Vector2 itemMin = item.bounds.min;
            Vector2 itemMax = item.bounds.max;

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
            if (sr == null || sr.sprite == null) continue;
            
            bool inBox = (itemMin.x >= checkMin.x && itemMax.x <= checkMax.x && itemMin.y >= checkMin.y && itemMax.y <= checkMax.y);
            
            GameObject obj;
            Vector2 realCenter;
            
            if (inBox)
            {
                obj = Instantiate(item.gameObject);
                SpriteRenderer newSr = obj.GetComponent<SpriteRenderer>();
                newSr.sortingOrder = sr.sortingOrder + 1;
            
                realCenter = obj.transform.position; 
            }
            else
            {
                Sprite clippedSprite = ClipSprite(sr, clippedMin, clippedMax);
                if (clippedSprite == null) continue;

                obj = Instantiate(item.gameObject);
                SpriteRenderer newSr = obj.GetComponent<SpriteRenderer>();
                newSr.sprite = clippedSprite;
                newSr.sortingOrder = sr.sortingOrder + 1;

                realCenter = (clippedMin + clippedMax) * 0.5f;
                obj.transform.position = realCenter;

                ReplaceClippedCollider(item.gameObject ,obj, clippedMin, clippedMax);
            }

            obj.GetComponent<Collider2D>().enabled = false;
            _copyObjs.Add(item.gameObject, new CopyedObj(realCenter - pos, obj));
        }

        if (_copyObjs.Count == 0)
            return;

        _copying = true;
    }

    private void ReplaceClippedCollider(GameObject originalItem, GameObject copiedObj, Vector2 checkMin, Vector2 checkMax)
    {
        // 1. 복사된 객체의 기존 콜라이더 싹 지우기
        foreach (var col in copiedObj.GetComponents<Collider2D>()) Destroy(col);

        Collider2D originalCol = originalItem.GetComponent<Collider2D>();
        if (originalCol == null) return;

        // 2. 원본 콜라이더의 모양을 월드 좌표계의 다각형 점(Points)들로 변환
        List<List<Vector2>> originalPaths = GetWorldPathsFromCollider(originalCol);
        
        PolygonCollider2D polyCol = null;
        int pathIndex = 0;

        foreach (var path in originalPaths)
        {
            // 3. 카메라 영역(checkMin, checkMax)을 칼날 삼아 다각형을 잘라냅니다.
            List<Vector2> clippedWorldPath = ClipPolygonAgainstAABB(path, checkMin, checkMax);

            // 잘린 후 남은 점이 3개 이상이어야 면(다각형)이 성립됨
            if (clippedWorldPath.Count > 2) 
            {
                if (polyCol == null) polyCol = copiedObj.AddComponent<PolygonCollider2D>();
                
                polyCol.pathCount = pathIndex + 1;
                
                // 4. 잘려진 월드 좌표 점들을 복사된 객체의 로컬 좌표로 변환해서 삽입
                Vector2[] localPath = new Vector2[clippedWorldPath.Count];
                for (int i = 0; i < clippedWorldPath.Count; i++)
                {
                    localPath[i] = copiedObj.transform.InverseTransformPoint(clippedWorldPath[i]);
                }
                
                polyCol.SetPath(pathIndex, localPath);
                pathIndex++;
            }
        }
    }
    
    // 다각형을 카메라의 상하좌우 4개의 선을 기준으로 순차적으로 잘라냅니다.
    private List<Vector2> ClipPolygonAgainstAABB(List<Vector2> poly, Vector2 min, Vector2 max)
    {
        poly = ClipEdge(poly, 0, min.x); // 왼쪽 자르기
        poly = ClipEdge(poly, 1, max.x); // 오른쪽 자르기
        poly = ClipEdge(poly, 2, min.y); // 아래쪽 자르기
        poly = ClipEdge(poly, 3, max.y); // 위쪽 자르기
        return poly;
    }

    private List<Vector2> ClipEdge(List<Vector2> poly, int edge, float value)
    {
        if (poly.Count == 0) return poly;
        List<Vector2> clipped = new List<Vector2>();
        Vector2 prev = poly[poly.Count - 1];
        bool prevInside = IsInside(prev, edge, value);

        foreach (Vector2 curr in poly)
        {
            bool currInside = IsInside(curr, edge, value);
            
            // 선을 가로지를 때 교차점 계산
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
        return edge switch {
            0 => p.x >= value, // Left
            1 => p.x <= value, // Right
            2 => p.y >= value, // Bottom
            3 => p.y <= value, // Top
            _ => false
        };
    }

    private Vector2 GetIntersection(Vector2 p1, Vector2 p2, int edge, float value)
    {
        if (edge == 0 || edge == 1) // 수직선 교차
        {
            float t = (value - p1.x) / (p2.x - p1.x);
            return new Vector2(value, p1.y + t * (p2.y - p1.y));
        }
        else // 수평선 교차
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
                foreach (Vector2 p in path) worldPath.Add(t.TransformPoint(p + poly.offset));
                paths.Add(worldPath);
            }
        }
        else if (col is BoxCollider2D box)
        {
            Vector2 halfSize = box.size * 0.5f;
            Vector2 offset = box.offset;
            List<Vector2> worldPath = new List<Vector2> {
                t.TransformPoint(offset + new Vector2(-halfSize.x, -halfSize.y)),
                t.TransformPoint(offset + new Vector2(halfSize.x, -halfSize.y)),
                t.TransformPoint(offset + new Vector2(halfSize.x, halfSize.y)),
                t.TransformPoint(offset + new Vector2(-halfSize.x, halfSize.y))
            };
            paths.Add(worldPath);
        }
        else if (col is CircleCollider2D circle)
        {
            List<Vector2> worldPath = new List<Vector2>();
            int segments = 24; // 원을 24각형으로 근사치 계산
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
        float radius = (capsule.direction == CapsuleDirection2D.Vertical ? size.x : size.y) * 0.5f;
        float height = (capsule.direction == CapsuleDirection2D.Vertical ? size.y : size.x);
        float straightLen = Mathf.Max(0, height - radius * 2);
        Vector2 offset = capsule.offset;

        if (capsule.direction == CapsuleDirection2D.Vertical)
        {
            Vector2 topCenter = offset + new Vector2(0, straightLen * 0.5f);
            Vector2 botCenter = offset + new Vector2(0, -straightLen * 0.5f);

            for (int i = 0; i <= segments; i++) {
                float angle = Mathf.Lerp(0, Mathf.PI, (float)i / segments);
                points.Add(t.TransformPoint(topCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius));
            }
            for (int i = 0; i <= segments; i++) {
                float angle = Mathf.Lerp(Mathf.PI, Mathf.PI * 2, (float)i / segments);
                points.Add(t.TransformPoint(botCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius));
            }
        }
        else // 가로 캡슐
        {
            Vector2 rightCenter = offset + new Vector2(straightLen * 0.5f, 0);
            Vector2 leftCenter = offset + new Vector2(-straightLen * 0.5f, 0);

            for (int i = 0; i <= segments; i++) {
                float angle = Mathf.Lerp(-Mathf.PI/2, Mathf.PI/2, (float)i / segments);
                points.Add(t.TransformPoint(rightCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius));
            }
            for (int i = 0; i <= segments; i++) {
                float angle = Mathf.Lerp(Mathf.PI/2, Mathf.PI * 1.5f, (float)i / segments);
                points.Add(t.TransformPoint(leftCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius));
            }
        }
        return points;
    }

    private Sprite ClipSprite(SpriteRenderer sr, Vector2 worldMin, Vector2 worldMax)
    {
        Sprite original = sr.sprite;
        Texture2D originalTex = original.texture;

        Texture2D readableTex = GetReadableTexture(originalTex);

        float ppu = original.pixelsPerUnit;

        Vector2 objWorldMin = sr.bounds.min;
        Vector2 objWorldSize = sr.bounds.size;

        Rect spriteRect = original.textureRect;

        float u0 = (worldMin.x - objWorldMin.x) / objWorldSize.x;
        float v0 = (worldMin.y - objWorldMin.y) / objWorldSize.y;
        float u1 = (worldMax.x - objWorldMin.x) / objWorldSize.x;
        float v1 = (worldMax.y - objWorldMin.y) / objWorldSize.y;

        int px = Mathf.RoundToInt(spriteRect.x + u0 * spriteRect.width);
        int py = Mathf.RoundToInt(spriteRect.y + v0 * spriteRect.height);
        int pw = Mathf.RoundToInt((u1 - u0) * spriteRect.width);
        int ph = Mathf.RoundToInt((v1 - v0) * spriteRect.height);

        if (pw <= 0 || ph <= 0) return null;

        px = Mathf.Clamp(px, 0, readableTex.width - 1);
        py = Mathf.Clamp(py, 0, readableTex.height - 1);
        pw = Mathf.Clamp(pw, 1, readableTex.width - px);
        ph = Mathf.Clamp(ph, 1, readableTex.height - py);

        Color[] pixels = readableTex.GetPixels(px, py, pw, ph);
        Texture2D newTex = new Texture2D(pw, ph, TextureFormat.RGBA32, false);
        newTex.filterMode = originalTex.filterMode;
        newTex.SetPixels(pixels);
        newTex.Apply();

        Sprite newSprite = Sprite.Create(
            newTex,
            new Rect(0, 0, pw, ph),
            new Vector2(0.5f, 0.5f),
            ppu
        );

        return newSprite;
    }

    private Texture2D GetReadableTexture(Texture2D source)
    {
        RenderTexture rt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);
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

    private void CheckObj()
    {
        Collider2D item = Physics2D.OverlapBox(checkPos.position, (myPosition.sizeDelta * 0.009f), 0, coloredObject);

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
            Gizmos.DrawWireCube(checkPos.position, myPosition.sizeDelta * 0.009f);
        }
    }
}
