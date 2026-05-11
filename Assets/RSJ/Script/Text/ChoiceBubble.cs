using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChoiceBubble : MonoBehaviour
{
    [SerializeField] private List<Button> _choiceBtns = new List<Button>();
    
    private List<TextMeshProUGUI> _choiceTexts = new List<TextMeshProUGUI>();
    
    private void Awake()
    {
        foreach (Button b in _choiceBtns)
        {
            _choiceTexts.Add(b.GetComponentInChildren<TextMeshProUGUI>());
        }
    }

    public void ChoiceInit(string[] choices)
    {
        for (int i = 0; i < _choiceTexts.Count; i++)
        {
            _choiceTexts[i].SetText(choices[i]);
        }
    }

    public void AddEvent(UnityAction<GameObject, int> actions)
    {
        for (int i = 0; i < _choiceBtns.Count; i++)
        {
            int index = i;

            _choiceBtns[index].onClick.AddListener(() => { actions(this.gameObject, index); });
        }
    }
}
