using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GetRealTime : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _time;

    private void Start()
    {
        StartCoroutine(GetTimer());
    }

    private IEnumerator GetTimer()
    {
        while(true)
        {
            _time.text = DateTime.Now.ToString("HH:mm");

            yield return new WaitForSecondsRealtime(1f);
        }
    }
}
