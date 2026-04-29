using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GetRealTime : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _time;
    [SerializeField] private TextMeshProUGUI _day;

    private void Start()
    {
        StartCoroutine(GetTimer());
    }

    private IEnumerator GetTimer()
    {
        while(true)
        {
            _time.text = DateTime.Now.ToString("HH:mm");
            _day.text = DateTime.Now.ToString("M¿ù ddÀÏ dddd");

            yield return new WaitForSecondsRealtime(1f);
        }
    }
}
