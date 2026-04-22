using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Transform checkPos;
    [SerializeField] private LayerMask coloredObject;
    [SerializeField] private Image _img;

    private RectTransform myPosition;
    private Vector3 _position;
    private MyColor _nowColor;
    
    private void Awake()
    {
        myPosition = GetComponent<RectTransform>();
        _nowColor = MyColor.None;
    }

    private void Update()
    {
        _position = new Vector3(Input.mousePosition.x - (myPosition.sizeDelta.x / 2), Input.mousePosition.y - (myPosition.sizeDelta.y / 2));
        myPosition.anchoredPosition = _position;

        //Test
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            _img.color = new Color(1, 1, 1, 1);
            CheckObject();
            _img.DOFade(0f, 0.2f);
            Debug.Log("ÂûÄ¬");
        }
    }

    private void CheckObject()
    {    
        Collider2D item = Physics2D.OverlapBox(checkPos.position, (myPosition.sizeDelta / 110), 0, coloredObject);
        
        if (item != null && item.gameObject.TryGetComponent(out ColoredObject colorObj))
        {
            _nowColor = colorObj.color;
        }
    }
}
