using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    private bool _canScreenChangeState = false;
    private bool _canThemaChangeState = false;
    private bool _canSpeedChangeState = false;

    public void SetScreenSize(Object textObj)
    {
        if(_canScreenChangeState == false)
        {
            _canScreenChangeState = true;
            return;
        }

        string text = textObj.GetComponent<TextMeshProUGUI>().text;

        if(text == "전체 화면")
        {
            Debug.Log("전체");
            Screen.SetResolution(1920, 1080, true);
        }
        else
        {
            Debug.Log("창");
            Screen.SetResolution(1920, 1080, false);
        }

        _canScreenChangeState = false;
    }

    public void SetThema(Object textObj)
    {
        if (_canThemaChangeState == false)
        {
            _canThemaChangeState = true;
            return;
        }

        string text = textObj.GetComponent<TextMeshProUGUI>().text;

        if (text == "다크")
        {
            Debug.Log("닼");
        }
        else
        {
            Debug.Log("화이트");
        }

        _canThemaChangeState = false;
    }
    
    public void SetTextSpeed(Object textObj)
    {
        if (_canSpeedChangeState == false)
        {
            _canSpeedChangeState = true;
            return;
        }

        string text = textObj.GetComponent<TextMeshProUGUI>().text;

        if (text == "보통")
        {
            Debug.Log("봍ㅗㅇ");
        }
        else if(text == "빠르게")
        {
            Debug.Log("빠르게");
        }
        else
        {
            Debug.Log("느리게");
        }

        _canSpeedChangeState = false;
    }
}