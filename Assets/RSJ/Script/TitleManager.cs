using DG.Tweening;
using LSW._02._Code.System___Manager;
using Moon._01.Script.Datas;
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

    [Header("Day")]
    [SerializeField] private TextMeshProUGUI _textsObj;
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
    private int _day = 1;

    private BubbleManager endAction;


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

    private void Start()
    {
        endAction.onEndChat += ActiveBtn;
    }

    public void ChangeDay()
    {
        _day++;
        _textsObj.SetText($"DAY {_day}");
        DataManager.Instance.SaveData("Day", _day);
        _candleBtn.interactable = false;
        _candleLight.SetActive(false);
        StartCoroutine(StartFade());
    }

    public void ActiveBtn(Button btn)
    {
        btn.interactable = true;
    }

    private void ActiveBtn()
    {
        if(_day <=10)
        {
            _candleBtn.interactable = true;
        }
        else
        {
            _teaBtn.interactable = true;
        }
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
            .OnComplete(() => ChangeCorrectScene(_day));
    }

    private void ChangeCorrectScene(int day)
    {
        switch (day % 5)
        {
            case 1:
                SceneManager.LoadScene("LeeJaeYoon");
                break;
            case 2:
                SceneManager.LoadScene("ChoiMyeongJin");
                break;
            case 3:
                SceneManager.LoadScene("ParkYool");
                break;
            case 4:
                SceneManager.LoadScene("YoonSeoAh");
                break;
            case 0:
                SceneManager.LoadScene("ChoiMyeongJin");
                break;
        }
    }

    private IEnumerator StartFade()
    {
        yield return new WaitForSeconds(0.2f);

        _fadeImg.DOFade(1f, 0.4f)
            .OnComplete(() => _textsObj.gameObject.SetActive(true));
    }

    private void Update()
    {
        FlickCandle();

        if (_prevStack.Count > 0)
            _leftBtn.interactable = true;
        if (_nextStack.Count > 0)
            _rightBtn.interactable = true;

        if (_textsObj.gameObject.activeSelf == false) return;

        timer += Time.deltaTime;

        if (timer > 1f)
        {
            _candleLight.SetActive(true);
            SetCalender();
            timer = 0f;
            _textsObj.gameObject.SetActive(false);
            _fadeImg.color = new Color(0f, 0f, 0f, 0);
        }
    }

    private void SetCalender()
    {
        _gameDayText.SetText($"DAY {_day}");
        _canlenderNumberText.SetText((_day + 26 % 31) == 0 ? "1" : $"{ (_day + 26 % 31)}");
        _canlenderDayText.SetText(CalculDay(_day / 7));
        _canlenderMonthText.SetText(_day < 5 ? "Oct" : "Nov");
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

    private void OnDestroy()
    {
        endAction.onEndChat -= ActiveBtn;
    }
}
