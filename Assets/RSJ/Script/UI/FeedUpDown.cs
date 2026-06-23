using DG.Tweening;
using LSW._02._Code.Core;
using LSW._02._Code.Core.Cores;
using UnityEngine;

public class FeedUpDown : MonoBehaviour
{
    [SerializeField] private RectTransform FeedContainer;
    [SerializeField] private GameObject UpBtn;
    [SerializeField] private GameObject DownBtn;

    private int nowIndex = 0;
    private bool isEnd = true;

    public int feedCount { get; set; } = 0;

    private void Start()
    {
        UpBtn.SetActive(false);
        DownBtn.SetActive(false);
        isEnd = true;
        ChangeFeedwithDay();
    }

    public void ChangeFeedwithDay()
    {
        feedCount++;
        if (4 >= feedCount)
            DownBtn.SetActive(true);
    }

    public void UpFeed()
    {
        if (isEnd == false) 
            return; 
        nowIndex--;

        isEnd = false;

        if (nowIndex <= 0) 
            UpBtn.SetActive(false);
        DownBtn.SetActive(true);

        FeedContainer.DOAnchorPosY(FeedContainer.anchoredPosition.y - 907.205f, 0.2f)
            .OnComplete(() => isEnd = true);
    }

    public void DownFeed()
    {
        if (isEnd == false) return;

        nowIndex++;
        isEnd = false;

        int maxIndex = CoreHandler.Instance.GetCore<GameStatueCore>().CurrentDay - 2;
        if(nowIndex >= maxIndex) 
            DownBtn.SetActive(false);
        UpBtn.SetActive(true);

        FeedContainer.DOAnchorPosY(FeedContainer.anchoredPosition.y + 907.205f, 0.2f)
            .OnComplete(() => isEnd = true);
    }
}
