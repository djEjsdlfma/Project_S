using System;
using csiimnida.CSILib.SoundManager.RunTime;
using LSW._02._Code.Core.Cores;
using LSW._02._Code.System___Manager;
using LSW._02._Code.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Feed : MonoBehaviour
{
    [SerializeField] private Guest guest;
    [SerializeField] private Image alarmImage;
    
    [SerializeField] private ChattingUIManager chattingUIManager;
    [SerializeField] private GuestProfile profile;
    
    [SerializeField] private TextMeshProUGUI btnText;

    [SerializeField] private TextMeshProUGUI HeartText;
    [SerializeField] private TextMeshProUGUI BookMarkText;

    [SerializeField] private GameObject _info;
    [SerializeField] private Button _btn;

    private static bool _isUploaded;
    public int heart;
    public int bookmark;

    private UnityAction _onUpload;
    private BubbleManager _bubbleManager;
    
    private void Awake()
    {
        DontDestroyOnLoad(HeartText);
        DontDestroyOnLoad(BookMarkText);

        _bubbleManager = SystemManager.Instance.GetSystemManager<BubbleManager>();
        if(_bubbleManager == null)
            return;
        
        _onUpload = () =>
        {
            _bubbleManager.ChatProfileContainer.ChangeProfileToActivable(guest);
            ShowProfile();
            Upload();
        };
        _btn.onClick.AddListener(_onUpload);
    }

    private void Start()
    {
        if(_isUploaded == false)
            _info.SetActive(false);
    }

    public void Upload(bool showAlarm = true)
    {
        SoundManager.Instance.PlaySound("FeedUp");
        _isUploaded = true;
        _info.SetActive(true);
        btnText.text = "게시 됨";
        _btn.interactable = false;
        alarmImage.gameObject.SetActive(showAlarm);
    }

    private void ShowProfile()
    {
        chattingUIManager.ShowProfil(profile.gameObject);
    }

    public void OnUploadClickImmediately()
    { 
        _bubbleManager.ChatProfileContainer.ChangeProfileToActivable(guest);
        ShowProfile();
        Upload(false);
    }

    public void OnEnable()
    {
        if (_btn.interactable == false)
        {
            HeartText.SetText($"{heart}");
            BookMarkText.SetText($"{bookmark}");
        }
    }

    private void OnDestroy()
    {
        _btn.onClick.RemoveListener(_onUpload);
    }
}
