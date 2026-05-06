using UnityEngine;

[CreateAssetMenu(fileName = "ItemTempSO", menuName = "Temp/ItemTempSO",order = 10)]
public class ItemTempSO : ScriptableObject
{
    public Sprite iconImg;

    public string Name;
    [TextArea]
    public string Desc;
    public int money;
}
