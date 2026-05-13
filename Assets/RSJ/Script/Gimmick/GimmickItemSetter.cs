using System;
using UnityEngine;

public class GimmickItemSetter : MonoBehaviour
{
    //승원아 나중에 알아서 정리 ㄱ

    [SerializeField] private GimmickItemManager[] items;

    public void Awake()
    {
        foreach(GimmickItemManager item in items)
        {
            for(int i = 0; i < item.gimmick.Length; i++)
            {
                item.gimmick[i].SetNeedItemId(item.needItem._itemId);
            }
        }
    }
}

[Serializable]
public struct GimmickItemManager
{
    //맞는 아이템
    public Item needItem;

    // 그 아이템에 해당되는 오브젝트
    public GimmickObj[] gimmick;


}

