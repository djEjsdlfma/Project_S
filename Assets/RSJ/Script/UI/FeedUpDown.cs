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

        int currentDay = CoreHandler.Instance.GetCore<GameStatueCore>().CurrentDay;
        feedCount = Mathf.Clamp(currentDay, 1, 5); 

        DownBtn.SetActive(feedCount > 1);
    }

    public void ChangeFeedwithDay()
    {
        if (feedCount < 5)
        {
            feedCount++;
        }

        if (feedCount > 1) 
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
        if (!isEnd) return;
        
        if (nowIndex < feedCount - 1)
        {
            nowIndex++;
            isEnd = false;

            UpBtn.SetActive(true);
            if (nowIndex >= feedCount - 1)
                DownBtn.SetActive(false);

            FeedContainer.DOAnchorPosY(FeedContainer.anchoredPosition.y + 907.205f, 0.2f)
                .OnComplete(() => isEnd = true);
        }
    }
}
