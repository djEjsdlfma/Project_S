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
        _position = new Vector3(Input.mousePosition.x - (myPosition.sizeDelta.x / 2), Input.mousePosition.y - (myPosition.sizeDelta.y / 2));
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

    private void StopCopy(bool b = true)
    {
        _copying = false;
        if (b)
        {
            foreach (var camObj in _copyObjs)
            {
                if(camObj.Value._copyObj)
                {
                    Destroy(camObj.Value._copyObj);
                }
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

            if (itemMin.x < checkMin.x || itemMin.y < checkMin.y ||
                itemMax.x > checkMax.x || itemMax.y > checkMax.y)
                continue;

            GameObject obj = Instantiate(item.gameObject);
            obj.transform.position = item.transform.position;
            _copyObjs.Add(item.gameObject, new CopyedObj((Vector2)item.transform.position - pos, obj));
        }

        if (_copyObjs.Count == 0)
            return;

        _copying = true;
    }

    private void PressObj()
    {
        StopCopy(false);
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
