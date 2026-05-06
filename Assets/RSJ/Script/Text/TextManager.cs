using UnityEngine;

public class TextManager1 : MonoBehaviour
{
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
