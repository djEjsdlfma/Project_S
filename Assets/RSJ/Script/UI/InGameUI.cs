using UnityEngine;
using UnityEngine.InputSystem;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private GameObject _pauseSettingUI;

    private void Update()
    {
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            _pauseSettingUI.SetActive(!_pauseSettingUI.activeSelf);
        }
    }
}
