using System;
using csiimnida.CSILib.SoundManager.RunTime;
using LSW._02._Code.System___Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Feed : MonoBehaviour
{
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
            _bubbleManager.ChatProfileContainer.ChangeProfileToActivable();
        };
        _btn.onClick.AddListener(_onUpload);
    }

    private void Start()
    {
        if(_isUploaded == false)
            _info.SetActive(false);
    }

    public void Upload()
    {
        SoundManager.Instance.PlaySound("FeedUp");
        _isUploaded = true;
        _info.SetActive(true);
        btnText.text = "게시 됨";
        _btn.interactable = false;
    }

    public void OnUploadClickImmediately()
    {
        _btn.onClick.Invoke();
        Upload();
    }

    public void OnEnable()
    {
        if (_btn.interactable == false)
        {
            HeartText.SetText($"{heart}");
            BookMarkText.SetText($"{bookmark}");
        }
    }

    private void OnDisable()
    {
        // if(_bubbleManager == null)
        //     return;
        // _btn.onClick.RemoveListener(_onUpload);
    }
}
