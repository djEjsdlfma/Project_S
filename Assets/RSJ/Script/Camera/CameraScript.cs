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
        Collider2D[] items = Physics2D.OverlapBoxAll(checkPos.position, (myPosition.sizeDelta / 110), 0, coloredObject);

        if (items == null) return;

        Vector2 checkMin = (Vector2)checkPos.position - myPosition.sizeDelta / 110 / 2;
        Vector2 checkMax = (Vector2)checkPos.position + myPosition.sizeDelta / 110 / 2;

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

                realCenter = (clippedMin + clippedMax) / 2f;
                obj.transform.position = realCenter;

                ReplaceClippedCollider(obj, clippedMin, clippedMax);
            }

            obj.GetComponent<Collider2D>().enabled = false;
            _copyObjs.Add(item.gameObject, new CopyedObj(realCenter - pos, obj));
        }

        if (_copyObjs.Count == 0)
            return;

        _copying = true;
    }

    private void ReplaceClippedCollider(GameObject obj, Vector2 worldMin, Vector2 worldMax)
    {
        foreach (var col in obj.GetComponents<Collider2D>())
            DestroyImmediate(col);

        BoxCollider2D newCol = obj.AddComponent<BoxCollider2D>();
        newCol.size = obj.transform.InverseTransformVector(worldMax - worldMin);
        newCol.offset = Vector2.zero;
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
        Collider2D item = Physics2D.OverlapBox(checkPos.position, (myPosition.sizeDelta / 110), 0, coloredObject);

        if (item != null && item.gameObject.TryGetComponent(out ColoredObject colorObj))
        {
            _nowColor = colorObj.color;
        }
    }
}
