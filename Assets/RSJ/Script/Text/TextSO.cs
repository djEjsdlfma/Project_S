using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TextSO", menuName = "SO/TextSO",order = 1)]
public class TextSO : ScriptableObject
{
    public List<string> _textList;
}
