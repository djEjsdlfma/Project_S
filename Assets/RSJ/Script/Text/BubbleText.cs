using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class BubbleText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _name;

    public string _myLog { get; set; }

    public float TextSpeed { get; set; }

    public void InitBubble(string log, float speed, string name = null)
    {
        _myLog = log;
        TextSpeed = speed;

        if (name != null)
            _name.text = name;

        _text.text = _myLog;
    }


    public void DoTextWithTMP(TextMeshProUGUI tmp, float duration)
    {
        tmp.maxVisibleCharacters = 0;
        DOTween.To(x => tmp.maxVisibleCharacters = (int)x, 0f, tmp.text.Length, duration);
    }
}
