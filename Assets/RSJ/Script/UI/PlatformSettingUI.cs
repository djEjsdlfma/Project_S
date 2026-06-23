using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlatformSettingUI : MonoBehaviour
{
    [SerializeField] private GameObject SettingObj;

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Time.timeScale = SettingObj.activeSelf ? 1f : 0f;
            SettingObj.SetActive(!SettingObj.activeSelf);
            Cursor.visible = SettingObj.activeSelf;
        }

#if UNITY_EDITOR
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            Time.timeScale = SettingObj.activeSelf ? 1f : 0f;
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

    public void ResetPlatform()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
