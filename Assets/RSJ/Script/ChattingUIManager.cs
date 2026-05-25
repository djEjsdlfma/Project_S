using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class ChattingUIManager : MonoBehaviour
{
    [SerializeField] private RectMask2D _profilBound;
    [SerializeField] private GameObject _chatAlarm;

    private bool chattingState;

    private void Awake()
    {
        // _profilBound.padding = (new Vector4(-1f, -600f, -1000f, -88f));
    }

    public void SetChatting(Transform myGameObj)
    {
        chattingState = true;
        myGameObj.parent.SetAsFirstSibling();
        // _profilBound.padding = (new Vector4(-1f, 65f, -1000f, -60f));
    }

    public void BackToMenu()
    {
        
    }

    public void ReadText()
    {
        _chatAlarm.SetActive(false);
    }

    public void ShowMyText(GameObject texts)
    {
        texts.SetActive(true);
    }
}
