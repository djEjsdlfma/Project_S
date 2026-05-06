using UnityEngine;

public class TextManager : MonoBehaviour
{
    [SerializeField] private ScriptSO _container1;

    private float TextSpeed;
    private float waitingTime;

    public void SetTextSpeed(float value)
    {
        TextSpeed = value;
    }

    public void SetWaitingTime(float value)
    {
        waitingTime = value;
    }
}
