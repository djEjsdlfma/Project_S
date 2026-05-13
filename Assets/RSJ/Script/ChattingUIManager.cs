using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class ChattingUIManager : MonoBehaviour
{
    [SerializeField] private RectMask2D _profilBound;

    private void Awake()
    {
        Debug.Log("?");
        _profilBound.padding = (new Vector4(-1f, -600f, -1000f, -88f));
    }

    public void SetChatting(Transform myGameObj)
    { 
        myGameObj.parent.SetAsFirstSibling();
        _profilBound.padding = (new Vector4(-1f, 65f, -1000f, -60f));
    }
}
