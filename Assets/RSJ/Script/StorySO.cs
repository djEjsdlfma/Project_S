using UnityEngine;

[CreateAssetMenu(fileName = "StorySO", menuName = "Story/StorySO")]
public class StorySO : ScriptableObject
{
    public ScriptSO[] Story;

    public ScriptReflectSO[] Choice;
}
