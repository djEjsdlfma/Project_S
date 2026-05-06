using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChoiceBubble : MonoBehaviour
{
    [SerializeField] private Button _choice1Btn;
    [SerializeField] private Button _choice2Btn;

    public void ChoiceInit(string cho1, string cho2)
    {
        _choice1Btn.GetComponentInChildren<TextMeshProUGUI>().text = cho1;
        _choice2Btn.GetComponentInChildren<TextMeshProUGUI>().text = cho2;
    }

    public void AddEvent(UnityAction<GameObject> action1, UnityAction<GameObject> action2)
    {
        _choice1Btn.onClick.AddListener(() => action1(this.gameObject));
        _choice2Btn.onClick.AddListener(() => action2(this.gameObject));
    }
}
