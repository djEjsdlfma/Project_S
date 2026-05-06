using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenOpenNClose : MonoBehaviour
{
    [SerializeField] private RectTransform application;
    [SerializeField] private RectTransform Pad;
    [SerializeField] private RectTransform PadCenter;

    private GameObject TurnOnApp;

    public void TurnOnApplication(RectTransform myIconInfo)
    {
        application.GetComponent<CanvasGroup>().DOFade(1f, 0f);
        application.DOKill();

        application.sizeDelta = myIconInfo.sizeDelta;
        application.anchoredPosition = myIconInfo.anchoredPosition;

        application.gameObject.SetActive(true);

        application.DOAnchorPos(PadCenter.anchoredPosition, 0.3f).SetEase(Ease.OutExpo)
            .OnUpdate(() => application.DOSizeDelta(Pad.sizeDelta, 1f).SetEase(Ease.OutExpo))
            .OnComplete(() => StartCoroutine(ShowApplication()));
    }

    public void OffApp(GameObject App)
    {
        App.SetActive(false);
    }

    public void SetMyApp(GameObject App)
    {
        TurnOnApp = App;
    }

    private IEnumerator ShowApplication()
    {
        yield return new WaitForSecondsRealtime(0.6f);

        application.GetComponent<CanvasGroup>().DOFade(0f, 0.3f);
        TurnOnApp.SetActive(true);
    }
}
