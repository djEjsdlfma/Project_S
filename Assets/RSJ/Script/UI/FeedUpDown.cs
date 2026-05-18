using DG.Tweening;
using UnityEngine;

public class FeedUpDown : MonoBehaviour
{
    [SerializeField] private RectTransform FeedContainer;
    [SerializeField] private GameObject UpBtn;
    [SerializeField] private GameObject DownBtn;

    private int nowIndex = 0;
    private bool isEnd = true;

    private void Start()
    {
        UpBtn.SetActive(false);
        isEnd = true;
    }

    public void UpFeed()
    {
        if (isEnd == false) return; 
        nowIndex--;

        isEnd = false;

        if (nowIndex <= 0) UpBtn.SetActive(false);
        if (UpBtn.activeSelf == false) DownBtn.SetActive(true);

        FeedContainer.DOAnchorPosY(FeedContainer.anchoredPosition.y - 907.205f, 0.2f)
            .OnComplete(() => isEnd = true);
    }

    public void DownFeed()
    {
        if (isEnd == false) return;

        nowIndex++;
        isEnd = false;

        if(nowIndex >= 4) DownBtn.SetActive(false);
        if (UpBtn.activeSelf == false) UpBtn.SetActive(true);

        FeedContainer.DOAnchorPosY(FeedContainer.anchoredPosition.y + 907.205f, 0.2f)
            .OnComplete(() => isEnd = true);
    }
}
