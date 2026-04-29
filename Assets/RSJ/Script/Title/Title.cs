using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField] private GameObject[] etcBackground;
    [SerializeField] private RectTransform _titleImg;
    [SerializeField] private RectTransform _mainBG;
    [SerializeField] private Image[] _passWord;
    [SerializeField] private RectTransform _padMain;

    [SerializeField, Range(0f, 1f)]
    private float moveTime = 1f;
    [SerializeField]
    private Ease easeGrpah = Ease.OutQuad;

    private static bool isStarted = false;
    private int inputTry;
    private bool moving = false;

    private void Start()
    {
        DOTween.SetTweensCapacity(500, 50);
        if (isStarted) return;

        for(int i = 0; i < etcBackground.Length; i++)
        {
            etcBackground[i].SetActive(false);
        }
        _titleImg.gameObject.SetActive(true);
    }

    private void Update()
    {
        //나중에 이 부분은 지울것
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            SceneManager.LoadScene(0);

        if (!_titleImg.gameObject.activeSelf || moving) return;

        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            _passWord[inputTry].color = Color.black;
            inputTry++;
        }

        if(inputTry >= 4)
        {
            moving = true;
            _titleImg.DOAnchorPosY(980f, 0.5f)
                .OnStart(() =>
                {
                    _mainBG.DOScale(Vector3.one, 0.5f).OnComplete(() =>
                    {
                        _padMain.DOAnchorPosX(-214.65f, moveTime).SetEase(easeGrpah);
                    });
                })
                .OnComplete(() => _titleImg.gameObject.SetActive(false));
        }
    }
}
