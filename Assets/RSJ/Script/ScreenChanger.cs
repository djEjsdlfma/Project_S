using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenChanger : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _resoultion;

    private void Awake()
    {
        _resoultion.ClearOptions();
    }

    private void Start()
    {
        List<string> options = new List<string>();

        options.Add("전체 화면");
        options.Add("창 화면");

        _resoultion.AddOptions(options);
        _resoultion.value = 0;
        _resoultion.RefreshShownValue();
    }

    public void SetResolution(int index)
    {
    }
}
