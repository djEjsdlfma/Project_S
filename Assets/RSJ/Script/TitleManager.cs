using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private Image _fadeImg;
    [SerializeField] private GameObject _candleLight;

    [SerializeField] private GameObject _textsObj;
    [SerializeField] private TextMeshProUGUI _dayText;

    [SerializeField] private RectTransform _teaImg;
    [SerializeField] private Image _sceneFadeImg;

    private float timer = 0f;

    public void ChangeDay()
    {
        _candleLight.SetActive(false);
        StartCoroutine(StartFade());
    }

    public void ChangeScene()
    {
        _teaImg.DOSizeDelta(new Vector2(12000f, 12000f), 0.95f);
        StartCoroutine(StartChangeScene());
    }

    private IEnumerator StartChangeScene()
    {
        yield return new WaitForSeconds(0.35f);

        _sceneFadeImg.DOFade(1f, 0.6f);
            //.OnComplete(() => _textsObj.SetActive(true));
    }

    private IEnumerator StartFade()
    {
        yield return new WaitForSeconds(0.2f);

        _fadeImg.DOFade(1f, 0.4f)
            .OnComplete(() => _textsObj.SetActive(true));
    }

    private void Update()
    {
        if (_textsObj.activeSelf == false) return;

        timer += Time.deltaTime;

        if (timer > 1f)
        {
            timer = 0f;
            _textsObj.SetActive(false);
            _fadeImg.color = new Color(0f, 0f, 0f, 0);
        }
    }
}
