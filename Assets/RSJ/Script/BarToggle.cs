using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BarToggle : MonoBehaviour
{
    [SerializeField] private Image Background;
    [SerializeField] private RectTransform _circle;
    [SerializeField] private Color OnBackgroundColor;
    [SerializeField] private Color OffBackgroundColor;

    private void Start()
    {
        ChangeUI(true);
    }

    public void ChangeUI(bool value)
    {
        Background.DOColor(value ? OnBackgroundColor : OffBackgroundColor, 0.1f);
        _circle.DOAnchorPosX(value ? 13f : -13f, 0.1f);
    }
}
