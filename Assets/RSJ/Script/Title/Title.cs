using System.Globalization;
using DG.Tweening;
using System.Runtime.InteropServices.ComTypes;
using LSW._02._Code.System___Manager;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour, ISystemManager
{
    [SerializeField] private GameObject[] etcBackground;
    [SerializeField] private RectTransform _titleImg;
    [SerializeField] private Image[] _passWord;
    [SerializeField] private RectTransform _padMain;
    [SerializeField] private GameObject _start;

    [SerializeField] private TextMeshProUGUI _guidText;

    [Header("Objs")]
    [SerializeField] private RectTransform Pad;
    [SerializeField] private Image PadShadow;
    [SerializeField] private RectTransform Calendar;
    [SerializeField] private Image CalendarShadow;
    [SerializeField] private RectTransform Candle;
    [SerializeField] private RectTransform Tea;

    [SerializeField, Range(0f, 1f)]
    private float moveTime = 1f;
    [SerializeField]
    private Ease easeGrpah = Ease.OutQuad;

    private static bool isStarted = false;
    private int inputTry;
    private bool moving = false;
    
    public bool canEnterPassword = true;

    public void Initialize(SystemManager systemManager) { }

    private void Start()
    {
        if(isStarted)
        {
            ShadowFade(PadShadow, 0f);
            ShadowFade(CalendarShadow, 0f);
        }

        if (isStarted) return;

        Pad.anchoredPosition = new Vector3(90f, 0f);
        Calendar.anchoredPosition = new Vector3(Screen.width + 15f, 368f);
        Candle.anchoredPosition = new Vector3(Screen.width + 15f, 175f);
        Tea.anchoredPosition = new Vector3(Screen.width + 15f, -299f);

        BlinkText(true);

        _start.SetActive(true);

        DOTween.SetTweensCapacity(500, 50);

        for(int i = 0; i < etcBackground.Length; i++)
        {
            etcBackground[i].SetActive(false);
        }
        _titleImg.gameObject.SetActive(true);
    }

    private void Update()
    {
        if(isStarted) return;

        if (!_titleImg.gameObject.activeSelf || moving) return;

        if (WaNumberKeyPressed() && canEnterPassword)
        {
            RectTransform nowImgPos;
            _passWord[inputTry].color = Color.gray3;

            nowImgPos = _passWord[inputTry].GetComponent<RectTransform>();
            nowImgPos.DOAnchorPosY(nowImgPos.anchoredPosition.y + 5f, 0.2f)
                .OnComplete(() => nowImgPos.DOAnchorPosY(nowImgPos.anchoredPosition.y - 5f, 0.2f));

            inputTry++;
        }

        if(inputTry >= 4)
        {
            moving = true;
            isStarted = true;
            _titleImg.DOAnchorPosY(980f, 0.5f)
                .OnComplete(() => 
                {
                    _titleImg.gameObject.SetActive(false);
                    SetObjects();
                });

        }
    }

    private bool WaNumberKeyPressed()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) 
            return false;
        
        for (int i = 0; i < 10; i++)
        {
            if (keyboard[Key.Digit0 + i].wasPressedThisFrame ||
                keyboard[Key.Numpad0 + i].wasPressedThisFrame)
            {
                return true;
            }
        }

        return false;
    }

    private void BlinkText(bool isTurntonInvisible)
    {
        _guidText.DOFade(isTurntonInvisible ? 0.5f : 1f, 2.5f)
            .OnComplete(() => BlinkText(!isTurntonInvisible));
    }

    private void SetObjects()
    {
        Pad.DOAnchorPosX(-430f, 0.6f).SetEase(Ease.OutCirc)
            .OnComplete(() =>
            {
                Calendar.DOAnchorPosX(188f,0.45f).SetEase(Ease.OutCirc).OnComplete(() => 
                {
                            ShadowFade(PadShadow, 0.25f);
                            ShadowFade(CalendarShadow, 0.25f);
                    Candle.DOAnchorPosX(677f, 0.45f).SetEase(Ease.OutCirc)
                        .OnComplete(() =>
                        {
                            Tea.DOAnchorPosX(540f, 0.45f).SetEase(Ease.OutCirc);
                        });
                });
            });
    }

    private void ShadowFade(Image shadow,float value)
    {
        shadow.DOFade(0.25f, value);
    }

    public void Reset() { }
}
