using DG.Tweening;
using UnityEngine;

public class FadeGImmickObj : MonoBehaviour
{
    [Range(0f, 10f)] public float FadeTime;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _spriteRenderer.DOFade(0f, FadeTime)
            .OnComplete(()=> gameObject.SetActive(false));
    }
}
