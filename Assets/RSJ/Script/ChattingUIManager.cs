using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(10)]
public class ChattingUIManager : MonoBehaviour
{
    [SerializeField] private RectMask2D _profilBound;
    [SerializeField] private GameObject _chatAlarm;

    [SerializeField] private GameObject[] _chats;

    private void Awake()
    {
        // _profilBound.padding = (new Vector4(-1f, -600f, -1000f, -88f));
    }

    public void SetChatting(Transform myGameObj)
    {
        myGameObj.parent.SetAsFirstSibling();
        // _profilBound.padding = (new Vector4(-1f, 65f, -1000f, -60f));
    }

    public void BackToMenu()
    {
        for(int i = 0; i < _chats.Length; i++)
        {
            _chats[i].SetActive(false);
        }
        // _profilBound.padding = (new Vector4(-1f, -600f, -1000f, -88f));
    }

    public void ReadText()
    {
        _chatAlarm.SetActive(false);
    }

    public void ShowMyText(GameObject texts)
    {
        texts.SetActive(true);
    }

    public void ShowProfil(GameObject profil)
    {
        _chatAlarm.SetActive(true);
        profil.SetActive(true);
    }
}
