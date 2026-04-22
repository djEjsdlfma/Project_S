using UnityEngine;

public enum MyColor
{
    None = -1,
    Red = 0,
    Green = 1,
    Blue = 2,
    Yellow = 3,
}

public class ColoredObject : MonoBehaviour
{
    [SerializeField] public MyColor color;
}
