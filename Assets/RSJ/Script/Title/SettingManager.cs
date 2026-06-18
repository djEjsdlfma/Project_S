using LSW._02._Code.System___Manager;
using Moon._01.Script.Datas;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    private bool _canScreenChangeState = false;
    private bool _canThemaChangeState = false;
    private bool _canSpeedChangeState = false;

    private BubbleManager _bubbleManager;
    private void Awake()
    {
        _bubbleManager = SystemManager.Instance.GetSystemManager<BubbleManager>();
    }

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
    
    public void SetTextSpeed(TextMeshProUGUI textObj)
    {
        if (_canSpeedChangeState == false)
        {
            _canSpeedChangeState = true;
            return;
        }

        string text = textObj.text;

        if (text == "보통")
        {
            DataManager.Instance.SaveData("TextSpeed", 4f);
            _bubbleManager.timerTreshold = 4f;
        }
        else if(text == "빠르게")
        {
            DataManager.Instance.SaveData("TextSpeed", 3f);
            _bubbleManager.timerTreshold = 3f;
        }
        else
        {
            DataManager.Instance.SaveData("TextSpeed", 5f);
            _bubbleManager.timerTreshold = 5f;
        }

        _canSpeedChangeState = false;
    }
}