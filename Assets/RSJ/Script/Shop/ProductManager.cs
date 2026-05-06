using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductManager : MonoBehaviour
{
    [SerializeField] private ItemTempSO[] ItemInfo;
    [SerializeField] private Product[] products;

    private void Awake()
    {
        for(int i = 0; i < ItemInfo.Length; i++)
        {
            products[i].SetInfo(ItemInfo[i]);
        }
    }

    public void Purchase(Button myButton)
    {
        myButton.interactable = false;
    }
}
