using System;
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

    public int heart;
    public int bookmark;

    private UnityAction _onUpload;
    private BubbleManager _bubbleManager;
    
    private void Awake()
    {
        _bubbleManager = SystemManager.Instance.GetSystemManager<BubbleManager>();
        if(_bubbleManager == null)
            return;

        _onUpload = () =>
        {
            Upload();
            _bubbleManager.ChatProfileContainer.ChangeProfileToActivable();
        };
        _btn.onClick.AddListener(_onUpload);
    }

    private void Start()
    {
        _info.SetActive(false);
    }

    public void Upload()
    {
        _info.SetActive(true);
        btnText.text = "게시 됨";
        _btn.interactable = false;
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
        if(_bubbleManager == null)
            return;
        _btn.onClick.RemoveListener(_onUpload);
    }
}
