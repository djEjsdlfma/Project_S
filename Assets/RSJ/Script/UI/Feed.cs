using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Feed : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI btnText;

    [SerializeField] private TextMeshProUGUI HeartText;
    [SerializeField] private TextMeshProUGUI BookMarkText;

    [SerializeField] private GameObject _info;
    [SerializeField] private Button _btn;

    private void Start()
    {
        _info.SetActive(false);
    }

    public void Upload()
    {
        _info.SetActive(true);
        btnText.text = "°³½Ã µÊ";
        _btn.interactable = false;
    }
}
