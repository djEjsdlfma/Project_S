using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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
    [SerializeField] private Button _leftBtn;
    [SerializeField] private Button _rightBtn;

    private Stack<GameObject> _prevStack = new Stack<GameObject>();
    private Stack<GameObject> _nextStack = new Stack<GameObject>();

    private float timer = 0f;
    private GameObject nowGameObjcet;

    private void Awake()
    {
        _leftBtn.interactable = false;
        _rightBtn.interactable = false;
    }

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
        if (nowGameObjcet != null)
        {
            _prevStack.Push(nowGameObjcet);
            _leftBtn.interactable = true;
            nowGameObjcet.SetActive(false);
        }

        if (_nextStack.Count > 0)
        {
            _rightBtn.interactable = false;
            _nextStack.Clear();
        }

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
        FlickCandle();

        if (_prevStack.Count > 0)
            _leftBtn.interactable = true;
        if (_nextStack.Count > 0)
            _rightBtn.interactable = true;

        if (_textsObj.activeSelf == false) return;

        timer += Time.deltaTime;

        if (timer > 1f)
        {
            timer = 0f;
            _textsObj.SetActive(false);
            _fadeImg.color = new Color(0f, 0f, 0f, 0);
        }
    }

    public void GotoPrev()
    {
        _nextStack.Push(nowGameObjcet);
        if(nowGameObjcet != null)
            nowGameObjcet.SetActive(false);
        
        nowGameObjcet = _prevStack.Peek();
        nowGameObjcet.SetActive(true);
        _prevStack.Pop();

        if (_prevStack.Count <= 0)
            _leftBtn.interactable = false;
    }

    public void GoToNext()
    {
        _prevStack.Push(nowGameObjcet);
        nowGameObjcet.SetActive(false);
        nowGameObjcet = _nextStack.Peek();
        nowGameObjcet.SetActive(true);
        _nextStack.Pop();


        if (_nextStack.Count <= 0)
            _rightBtn.interactable = false;
    }

    public void FlickCandle()
    {
        float noise = Mathf.PerlinNoise(Time.time * 0.7f, 0f);

        // 노이즈 값을 -1.0 ~ 1.0 사이로 정규화한 뒤 일렁임 범위를 곱합니다.
        float intensityFlicker = (noise - 0.5f) * 2f * 2.7f;

        if (_candleLight.activeSelf != false)
        {
            _candleLight.GetComponent<Light2D>().intensity = 11f + intensityFlicker;
        }
    }
}
