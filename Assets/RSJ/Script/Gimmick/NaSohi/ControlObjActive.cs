using TMPro;
using UnityEngine;

public class ControlObjActive : MonoBehaviour
{
    public float TimethresholdValue;

    private float timer = 0;
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer > TimethresholdValue)
        {
            timer = 0f;
            _collider.enabled = false;
            this.enabled = false;
        }
    }

    public void ActiveObj()
    {
        _collider.enabled = true;
        this.enabled = true;
    }
}
