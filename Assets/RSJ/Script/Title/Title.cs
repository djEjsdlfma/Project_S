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
using csiimnida.CSILib.SoundManager.RunTime;

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
    private Key[] numberKeys = new Key[]
    {
        Key.Digit0, Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, 
        Key.Digit5, Key.Digit6, Key.Digit7, Key.Digit8, Key.Digit9,
        Key.Numpad0, Key.Numpad1, Key.Numpad2, Key.Numpad3, Key.Numpad4, 
        Key.Numpad5, Key.Numpad6, Key.Numpad7, Key.Numpad8, Key.Numpad9
    };
    
    public bool canEnterPassword = true;

    public void Initialize(SystemManager systemManager) { }

    private void Start()
    {
        if(isStarted)
        {
            ShadowFade(PadShadow, 0f);
            ShadowFade(CalendarShadow, 0f);
        }

        for(int i = 0; i < etcBackground.Length; i++)
        {
            etcBackground[i].SetActive(false);
        }
        MainBgmPlayer.Instance.Play();

        if (isStarted) return;
        Pad.anchoredPosition = new Vector3(90f, 0f);
        Calendar.anchoredPosition = new Vector3(Screen.width + 15f, 368f);
        Candle.anchoredPosition = new Vector3(Screen.width + 15f, 175f);
        Tea.anchoredPosition = new Vector3(Screen.width + 15f, -299f);

        BlinkText(true);

        _start.SetActive(true);

        DOTween.SetTweensCapacity(500, 50);
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
            MainBgmPlayer.Instance.Play();
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
        
        for (int i = 0; i < numberKeys.Length; i++)
        {
            if (keyboard[numberKeys[i]].wasPressedThisFrame)
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
        SoundManager.Instance.PlaySound("TitleObjMove");
        Pad.DOAnchorPosX(-430f, 0.6f).SetEase(Ease.OutCirc)
            .OnComplete(() =>
            {
                SoundManager.Instance.PlaySound("TitleObjMove");
                Calendar.DOAnchorPosX(188f,0.45f).SetEase(Ease.OutCirc).OnComplete(() => 
                {
                    SoundManager.Instance.PlaySound("TitleObjMove");
                    ShadowFade(PadShadow, 0.25f);
                    ShadowFade(CalendarShadow, 0.25f);
                    Candle.DOAnchorPosX(677f, 0.45f).SetEase(Ease.OutCirc)
                        .OnComplete(() =>
                        {
                            SoundManager.Instance.PlaySound("TitleObjMove");
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
