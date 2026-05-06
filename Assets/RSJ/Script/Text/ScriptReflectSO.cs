using UnityEngine;

[CreateAssetMenu(fileName = "Script", menuName = "Text/Script")]
public class ScriptReflectSO : ScriptableObject
{
    public string _name;
    [TextArea] public string text;
    public bool isNPC;
    public ScriptEvent _event;

    public int ReactNum;
}
