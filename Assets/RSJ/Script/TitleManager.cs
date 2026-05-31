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

public class TitleManager : MonoBehaviour
{
    [Header("Candle")]
    [SerializeField] private GameObject _candleLight;

    [Header("Day")]
    [SerializeField] private TextMeshProUGUI _gameDayText;
    [SerializeField] private TextMeshProUGUI _canlenderNumberText;
    [SerializeField] private TextMeshProUGUI _canlenderDayText;
    [SerializeField] private TextMeshProUGUI _canlenderMonthText;

    [Header("tea")]
    [SerializeField] private RectTransform _teaImg;

    [Header("button")]
    [SerializeField] private Button _candleBtn;
    [SerializeField] private Button _teaBtn;
    [SerializeField] private Button _leftBtn;
    [SerializeField] private Button _rightBtn;

    private Stack<GameObject> _prevStack = new Stack<GameObject>();
    private Stack<GameObject> _nextStack = new Stack<GameObject>();

    private int _currentDay;
    private float timer = 0f;
    private GameObject nowGameObjcet;

    private GameStatueCore _gameStatueCore;
    private BubbleManager endAction;
    private bool _talkEnd;

    private static bool _textAgain;

    private void Awake()
    {
        if (DataManager.Instance.CurrentData.TryGetValue("Day", out int day))
        {
           //_day = day;
        }
        SetCalender();
        _leftBtn.interactable = false;
        _rightBtn.interactable = false;

        endAction = SystemManager.Instance.GetSystemManager<BubbleManager>();
    }
    
    private void OnCoreDayChanged()
    {
        // 코어의 현재값으로 동기화
        _currentDay = _gameStatueCore.CurrentDay;
        SetCalender();
    }

    private void Start()
    {
        _gameStatueCore = CoreHandler.Instance.GetCore<GameStatueCore>();
        if (_gameStatueCore != null)
        {
            _currentDay = _gameStatueCore.CurrentDay;
            _gameStatueCore.OnDayChanged += OnCoreDayChanged; // 구독
        }
        
        endAction.onEndChat += ActiveBtn;
        endAction.onEndChat += EndTalk;
    }

    public void ChangeDay()
    {
        _day++;
        _textsObj.SetText($"DAY {_day}");
        _textAgain = false;
        DataManager.Instance.SaveData("Day", _day);
        _candleBtn.interactable = false;
        _candleLight.SetActive(false);
        _talkEnd = false;
        StartCoroutine(StartFade());
    }

    public void ActiveBtn(Button btn)
    {
        btn.interactable = true;
    }

    private void ActiveBtn()
    {
        if(_currentDay <=10)
        {
            _candleBtn.interactable = true;
        }
        else
        {
            _textAgain = true;
            _teaBtn.interactable = true;
        }
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
    
    private void Update()
    {
        FlickCandle();

        if (_prevStack.Count > 0)
            _leftBtn.interactable = true;
        if (_nextStack.Count > 0)
            _rightBtn.interactable = true;
        
        timer += Time.deltaTime;

        if (timer > 1f)
        {
            _candleLight.SetActive(true);
            SetCalender();
            timer = 0f;
        }
        Debug.Log(_currentDay = _gameStatueCore.CurrentDay);
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
        if (_gameStatueCore != null)
            _gameStatueCore.OnDayChanged -= OnCoreDayChanged;
    }
}
