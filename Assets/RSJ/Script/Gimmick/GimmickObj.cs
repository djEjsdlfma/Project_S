using UnityEngine;
using UnityEngine.Events;

public class GimmickObj : MonoBehaviour
{
    private string NeedItemId;
    
    public UnityEvent _event;

    public void CheckItemId(string itemId)
    {
        if(NeedItemId == itemId)
        {
            _event?.Invoke();
        }
    }

    public void SetNeedItemId(string itemId)
    {
        NeedItemId = itemId;
    }
}

public enum EventType
{
   None = -1,

}
