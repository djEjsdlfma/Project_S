using System;
using UnityEngine;

public class ColorThemaManager : MonoBehaviour
{
    public static ColorThemaManager Instance;

    public DarkWhiteMode lightTheme;
    public DarkWhiteMode darkTheme;

    public static event Action OnThemeChanged;

    private bool isDark;
    public DarkWhiteMode Current => isDark ? darkTheme : lightTheme;

    void Awake()
    {
        Instance = this;
        isDark = PlayerPrefs.GetInt("DarkMode", 0) == 1;
    }

    public void Toggle()
    {
        isDark = !isDark;
        PlayerPrefs.SetInt("DarkMode", isDark ? 1 : 0);
        OnThemeChanged?.Invoke();
    }
}
