using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class BubbleText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private RectTransform _Boundery;
    [SerializeField] private RectTransform myChatBound;

    private float prevHeight;
    private float originHeight;

    public string _myLog { get; set; }

    public float TextSpeed { get; set; }

    private void Awake()
    {
        originHeight = _Boundery.sizeDelta.y;
        InitScale();
    }

    private void Update()
    {
        if (prevHeight < myChatBound.sizeDelta.y)
        {
            prevHeight = myChatBound.sizeDelta.y;
            InitScale();
        }
    }

    public void InitBubble(string log, float speed, string name = null)
    {
        _myLog = log;
        TextSpeed = speed;

        if (name != null)
            _name.text = name;

        _text.text = _myLog;
    }

    public void InitScale()
    {
        if(myChatBound.sizeDelta.y > 45)
        {
            Debug.Log(_Boundery.sizeDelta.y);

            _Boundery.sizeDelta = new Vector2(_Boundery.sizeDelta.x, originHeight + (23 * (myChatBound.sizeDelta.y / 40)));
        }
    }


    public void DoTextWithTMP(TextMeshProUGUI tmp, float duration)
    {
        tmp.maxVisibleCharacters = 0;
        DOTween.To(x => tmp.maxVisibleCharacters = (int)x, 0f, tmp.text.Length, duration);
    }
}
