using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChoiceBubble : MonoBehaviour
{
    [SerializeField] private List<Button> _choiceBtns = new List<Button>();
    [SerializeField] private RectTransform _selectionTrm;
    
    private List<TextMeshProUGUI> _choiceTexts = new List<TextMeshProUGUI>();

    private RectTransform _showTrm;
    private RectTransform _hideTrm;
    
    private void Awake()
    {
        _selectionTrm = GetComponent<RectTransform>();
        foreach (Button b in _choiceBtns)
        {
            _choiceTexts.Add(b.GetComponentInChildren<TextMeshProUGUI>());
        }
    }

    private void Start()
    {
        // _selectionTrm.anchoredPosition = _hideTrm.anchoredPosition;
    }

    public void ChoiceInit(string[] choices)
    {
        for (int i = 0; i < _choiceTexts.Count; i++)
        {
            _choiceTexts[i].SetText(choices[i]);
        }
        // _selectionTrm.DOMove(_showTrm.anchoredPosition, 0.2f);
    }

    public void AddEvent(UnityAction<GameObject, int> actions)
    {
        for (int i = 0; i < _choiceBtns.Count; i++)
        {
            int index = i;

            _choiceBtns[index].onClick.AddListener(() => { actions(this.gameObject, index); });
        }
    }

    public void Hide()
    {
        // _selectionTrm.DOMove(_hideTrm.anchoredPosition, 0.2f);
    }
}
