using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [Header("Candle")]
    [SerializeField] private Image _fadeImg;
    [SerializeField] private GameObject _candleLight;

    [Header("Calender")]
    [SerializeField] private GameObject _textsObj;
    [SerializeField] private TextMeshProUGUI _gameDayText;
    [SerializeField] private TextMeshProUGUI _canlenderNumberText;
    [SerializeField] private TextMeshProUGUI _canlenderDayText;
    [SerializeField] private TextMeshProUGUI _canlenderMonthText;

    [Header("tea")]
    [SerializeField] private RectTransform _teaImg;
    [SerializeField] private Image _sceneFadeImg;

    [Header("button")]
    [SerializeField] private Button _candleBtn;
    [SerializeField] private Button _teaBtn;

    private float timer = 0f;
    private GameObject nowGameObjcet;

    public void ChangeDay()
    {
        _candleBtn.interactable = false;
        _candleLight.SetActive(false);
        StartCoroutine(StartFade());
    }

    public void ActiveBtn(Button btn)
    {
        btn.interactable = true;
    }

    public void ChangeScene()
    {
        _teaBtn.interactable = false;
        _teaImg.DOSizeDelta(new Vector2(12000f, 12000f), 0.95f);
        StartCoroutine(StartChangeScene());
    }

    public void TurnOnPart(GameObject part)
    {
        if(nowGameObjcet != null)
            nowGameObjcet.SetActive(false);

        nowGameObjcet = part;
        part.SetActive(true);
    }

    private IEnumerator StartChangeScene()
    {
        yield return new WaitForSeconds(0.35f);

        _sceneFadeImg.DOFade(1f, 0.6f)
            .OnComplete(() => SceneManager.LoadScene("ChoiMyeongJin"));
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
