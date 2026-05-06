using System.Collections.Generic;
using UnityEngine;

public class ChoiceManager : MonoBehaviour
{
    [SerializeField] private List<ReactScript> AllScript;

    private Dictionary<int, string> ChoiceScriptDict;

    public static ChoiceManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

            foreach (ReactScript item in AllScript)
                ChoiceScriptDict[item.ReactNum] = item.text;
    }

    public string GetChoiceReact(int number)
    {
        return ChoiceScriptDict[number];
    }
}
