using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Product : MonoBehaviour
{
    [SerializeField] private ItemTempSO _myInfo;

    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _gold;
    [SerializeField] private TextMeshProUGUI _Desc;

    [SerializeField] private GameObject _parched;

    private void Start()
    {
        _icon.sprite = _myInfo.iconImg;

        _name.text = _myInfo.Name;
        _gold.text = _myInfo.money + " G";
        _Desc.text = _myInfo.Desc;
    }

    public void SetInfo(ItemTempSO item) => _myInfo = item;
}
