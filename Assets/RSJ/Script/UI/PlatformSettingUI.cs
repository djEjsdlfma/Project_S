using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformSettingUI : MonoBehaviour
{
    [SerializeField] private GameObject SettingObj;

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SettingObj.SetActive(!SettingObj.activeSelf);
            Cursor.visible = SettingObj.activeSelf;
        }

#if UNITY_EDITOR
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            SettingObj.SetActive(!SettingObj.activeSelf);
            Cursor.visible = SettingObj.activeSelf;
        }
#endif
    }

    public void OffSetting()
    {
        SettingObj.SetActive(false);
        Cursor.visible = false;
    }

    public void Debuging(string message)
    {
        Debug.Log(message);
    }
}
