
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TextManager : MonoBehaviour
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

    public void GetTextSpeed()
    {

    }

    public void GetWaitingSpeed()
    {

    }
}
