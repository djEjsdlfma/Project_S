using AYellowpaper.SerializedCollections;
using JetBrains.Annotations;
using LSW._02._Code.Core.Cores;
using Moon._01.Script.Datas;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(10)]
public class ChattingUIManager : MonoBehaviour
{
    [SerializeField] private RectMask2D _profilBound;
    [SerializeField] private GameObject _chatAlarm;

    [SerializeField] private SerializedDictionary<Guest, GameObject> _chats;

    private void Awake()
    {
        // _profilBound.padding = (new Vector4(-1f, -600f, -1000f, -88f));
    }

    private void Start()
    {
        if (DataManager.Instance.CurrentData.TryGetValue("Day", out int day) || day >= 6)
        {
            _chatAlarm.SetActive(true);
        }
    }

    public void SetChatting(Transform myGameObj)
    {
        myGameObj.parent.SetAsFirstSibling();
        // _profilBound.padding = (new Vector4(-1f, 65f, -1000f, -60f));
    }

    public void BackToMenu()
    {
        foreach (var chat in _chats)
        {
            chat.Value.SetActive(false);
        }
        // _profilBound.padding = (new Vector4(-1f, -600f, -1000f, -88f));
    }

    public void DisableChat(Guest guest, bool active)
    {
        if (guest == Guest.None)
        {
            return;
        }
        _chats[guest].SetActive(active);
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
