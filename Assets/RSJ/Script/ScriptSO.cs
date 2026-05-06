using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Container", menuName = "Text/ScriptContainer", order = 10)]
public class ScriptSO : ScriptableObject
{
    public Script[] scripts;

    public ChoiceScript[] cScript;

    public ReactScript[] reactScript;
}

[Serializable]
public class Script
{
    public string _name;
    [TextArea] public string text;
    public bool isNPC;
    public ScriptEvent _event;

    public int ChoiceNum;
}

[Serializable]
public class ReactScript
{
    public string _name;
    [TextArea] public string text;
    public bool isNPC;

    public int ReactNum;
}

[Serializable]
public class ChoiceScript
{
    public string choice1;
    public string choice2;

    public int ReactNum1;
    public int ReactNum2;

    public int ChoiceId;
}

public enum ScriptEvent
{ 
    None = 0,
    Normal,
    Choice,
    End
}
