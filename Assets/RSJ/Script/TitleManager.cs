using DG.Tweening;
using LSW._02._Code.System___Manager;
using Moon._01.Script.Datas;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using LSW._02._Code.Core;
using LSW._02._Code.Core.Cores;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using csiimnida.CSILib.SoundManager.RunTime;

public class TitleManager : MonoBehaviour
{
    [Header("Candle")]
    [SerializeField] private GameObject _candleLight;
    [SerializeField] private GameObject _candleFire;

    [Header("Day")]
    [SerializeField] private TextMeshProUGUI _gameDayText;
    [SerializeField] private TextMeshProUGUI _canlenderNumberText;
    [SerializeField] private TextMeshProUGUI _canlenderDayText;
    [SerializeField] private TextMeshProUGUI _canlenderMonthText;

    [Header("tea")]
    [SerializeField] private RectTransform _teaImg;
    [SerializeField] private Sprite[] _FillSprite;
    [SerializeField] private Image _teaFillImg;
    [SerializeField] private RectTransform _teaFillImgSize;

    [Header("button")]
    [SerializeField] private Button _candleBtn;
    [SerializeField] private Button _teaBtn;
    [SerializeField] private Button _leftBtn;
    [SerializeField] private Button _rightBtn;

    [SerializeField] private TextMeshProUGUI _flickText;
    [SerializeField] private GameObject _BlackImg;

    private Stack<GameObject> _prevStack = new Stack<GameObject>();
    private Stack<GameObject> _nextStack = new Stack<GameObject>();

    private int _currentDay;
    private float timer = 0f;
    private GameObject nowGameObjcet;

    private GameStatueCore _gameStatueCore;
    private BubbleManager endAction;
    private bool _talkEnd;

    private static bool _textAgain;

    private RectTransform _candleFireSize;

    private void Awake()
    {
        if (DataManager.Instance.CurrentData.TryGetValue("Day", out int day))
        {
           //_day = day;
        }


        _leftBtn.interactable = false;
        _rightBtn.interactable = false;

        endAction = SystemManager.Instance.GetSystemManager<BubbleManager>();
        _gameStatueCore = CoreHandler.Instance.GetCore<GameStatueCore>();
        _candleFireSize = _candleFire.GetComponent<RectTransform>();

        if (_gameStatueCore != null)
        {
            _currentDay = _gameStatueCore.CurrentDay;
        }
        SetCalender();
    }

    private void Start()
    {     
        endAction.onEndChat += ActiveBtn;
        endAction.onEndChat += EndTalk;
    }

    public void ActiveBtn(Button btn)
    {
        btn.interactable = true;
    }

    private void ActiveBtn()
    {
        if(_currentDay < 11)
        {
            _candleBtn.interactable = true;
            FlickText("촛불");
        }
        else
        {
            FlickText("찻잔");
            _textAgain = true;
            _teaBtn.interactable = true;
            FillTeaCup(_currentDay);
        }
    }

    public void TurnOnPart(GameObject part)
    {
        if (nowGameObjcet != null)
        {
            if(part != nowGameObjcet)
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
    
    private void Update()
    {
        FlickCandle();

        if (_prevStack.Count > 0)
            _leftBtn.interactable = true;
        if (_nextStack.Count > 0)
            _rightBtn.interactable = true;
       
    }

    private void SetCalender()
    {
        int day = _gameStatueCore != null ? _gameStatueCore.CurrentDay : _currentDay;

        _gameDayText.SetText($"DAY {day}");
        
        int calNum = (day + 26) % 31;
        if (calNum == 0) 
            calNum = 1;

        _canlenderNumberText.SetText(calNum.ToString());
        _canlenderDayText.SetText(CalculDay(day % 7));
        _canlenderMonthText.SetText(day < 5 ? "Oct" : "Nov");
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

        // ������ ���� -1.0 ~ 1.0 ���̷� ����ȭ�� �� �Ϸ��� ������ ���մϴ�.
        float intensityFlicker = (noise - 0.5f) * 2f * 2.7f;

        if (_candleLight.activeSelf != false)
        {
            _candleLight.GetComponent<Light2D>().intensity = 
                (_talkEnd ? 17f : 11f) + intensityFlicker;

            _candleFireSize.sizeDelta = new Vector2(_candleFireSize.sizeDelta.x, (intensityFlicker * 2) + 80f);
        }
    }

    private string CalculDay(int Day)
    {
        switch (Day)
        {
            case 4:
                return "MON";
            case 5:
                return "THE";
            case 6:
                return "WED";
            case 0:
                return "THU";
            case 1:
                return "FRI";
            case 2:
                return "SAT";
            case 3:
                return "SUN";
            default:
                return null;
        }
    }

    private void EndTalk() => _talkEnd = !_talkEnd;

    private void OnDestroy()
    {
        if (endAction != null)
        {
            endAction.onEndChat -= ActiveBtn;
            endAction.onEndChat -= EndTalk;
        }
    }

    private void FlickText(string objName, int value = 0)
    {
        _BlackImg.SetActive(true);
        if (objName == "촛불")
            _flickText.text = $"{objName}이 켜졌습니다.";
        else
            _flickText.text = $"{objName}이 채워졌습니다.";

        _flickText.DOFade(value % 2 == 0 ? 0.8f : 0f, 0.8f)
            .OnComplete(() => 
            {
                if (value <= 4)
                    FlickText(objName, ++value);
                else
                    _BlackImg.SetActive(false);
            });
    }

    public void FillTeaCup(int day)
    {
        SoundManager.Instance.PlaySound("CupFill");
        _teaFillImg.sprite = _FillSprite[day % 11];
        _teaFillImgSize.DOSizeDelta(new Vector2(100f, 100f), 0.5f);
    }

    public void CandleOff()
    {
        SoundManager.Instance.PlaySound("CandleOff");
        _candleLight.gameObject.SetActive(false);
        _candleFire.gameObject.SetActive(false);
    }
}
