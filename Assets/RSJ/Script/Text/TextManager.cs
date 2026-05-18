
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TextManager : MonoBehaviour
{
    private float TextSpeed;
    private float waitingTime;

    public void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            SceneManager.LoadScene(1);
    }

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
