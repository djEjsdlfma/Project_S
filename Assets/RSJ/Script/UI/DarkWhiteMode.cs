using UnityEngine;

[CreateAssetMenu(fileName = "Theme", menuName = "UI/Theme")]
public class DarkWhiteMode : ScriptableObject
{
    [System.Serializable]
    public class ColorEntry
    {
        public string key;     // "Background", "Text", "Button", "Accent", "Sub" ...
        public Color color;
    }

    public ColorEntry[] colors;

    // 키로 색 찾기
    public Color Get(string key)
    {
        foreach (var c in colors)
            if (c.key == key) return c.color;
        return Color.magenta;  // 못 찾으면 눈에 띄게 (디버깅용)
    }
}

public enum ThemeColor
{
    Background,
    Text,
    Button,
    Accent,
    Sub
}
