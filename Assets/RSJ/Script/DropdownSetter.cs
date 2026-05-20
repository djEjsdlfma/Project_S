using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class DropdownSetter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nowStateText;
    [SerializeField] private RectTransform arrowImg;
    [SerializeField] private RectTransform List;

    private bool isSetting = false;
    private float originHeight;

    private void Start()
    {
        originHeight = List.sizeDelta.y;
        List.sizeDelta = new Vector2(List.sizeDelta.x, 0f);
    }

    public void ChangeState(DropdownButton button)
    {
        nowStateText.text = button.SetAndGiveText(nowStateText.text);
    }

    public void ChangeSettingState()
    {
        isSetting = !isSetting;

        arrowImg.localScale = new Vector3(1, isSetting ? -1 : 1, 1);
        List.DOSizeDelta(new Vector2(List.sizeDelta.x, isSetting? originHeight : 0f), 0.1f);
    }
}
