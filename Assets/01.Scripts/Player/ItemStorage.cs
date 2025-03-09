using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemSlot
{
    public ItemStat itemInformation;
    public int itemAmount;
}

public class ItemStorage : MonoBehaviour
{
    public UI_Inventory ui;

    [SerializeField] private ItemSlot[] items;

    private Dictionary<ItemCategory, List<ItemSlot>> seperatedItems;
    public Dictionary<ItemCategory, List<ItemSlot>> SeperatedItems { get { return seperatedItems; } }

    void Start()
    {
        ui = FindAnyObjectByType<UI_Inventory>(FindObjectsInactive.Include);
        seperatedItems = new Dictionary<ItemCategory, List<ItemSlot>>();

        // 아무것도 없으면 카테고리 리스트는 생성해준다.
        int categoryCount = System.Enum.GetValues(typeof(ItemCategory)).Length;

        // init containter
        for(int c = 0; c< categoryCount; c++)
        {
            seperatedItems[(ItemCategory)c] = new List<ItemSlot>();
        }

        foreach(var item in items)
        {
            SetAmountOfItem(item.itemInformation, item.itemAmount);
        }
    }

    ItemSlot FindItem(ItemStat find)
    {
        var list = seperatedItems[find.category];

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].itemInformation.itemName == find.itemName)
            {
                return list[i];
            }
        }
        return null;
    }

   
    public int GetAmountOfItem(ItemStat find)
    {
        int amount = 0;

        var list = seperatedItems[find.category];

        for(int i = 0; i < list.Count; i++)
        {
            if (list[i].itemInformation.itemName == find.itemName)
            {
                amount = list[i].itemAmount;
            }
        }

        return amount;
    }

    public void SetAmountOfItem(ItemStat aquiredItem, int amount)
    {
        ItemSlot slot = FindItem(aquiredItem);
        if(slot is  null)  // new Item!!
        {
            slot = new ItemSlot();
            slot.itemInformation = aquiredItem;
            slot.itemAmount = amount;

            var itemList = seperatedItems[aquiredItem.category];
            itemList.Add(slot);
        }
        else
        {
            slot.itemAmount += amount;
        }

    }

}
