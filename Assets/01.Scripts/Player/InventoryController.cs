using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ItemSlot
{
    public ItemStat itemInformation;
    public int itemAmount;
}

public class InventoryController : MonoBehaviour
{
    UI_Inventory inventory_ui;

    PlayerController playerController;
    ItemCategory selectedCategory;

    [SerializeField] ItemSlot[] items;
    Dictionary<ItemCategory, List<ItemSlot>> seperatedItems;

    private void Awake()
    {
        inventory_ui = FindAnyObjectByType<UI_Inventory>(FindObjectsInactive.Include);
        TryGetComponent(out playerController);

        seperatedItems = new Dictionary<ItemCategory, List<ItemSlot>>();

        // 아무것도 없으면 카테고리 리스트는 생성해준다.
        int categoryCount = System.Enum.GetValues(typeof(ItemCategory)).Length;

        // init containter
        for(int c = 0; c< categoryCount; c++)
        {
            seperatedItems[(ItemCategory)c] = new List<ItemSlot>();
        }

        // seperate item
        foreach (var item in items)
        {
            seperatedItems[item.itemInformation.category].Add(item);
        }
    }

    void Update()
    {
        // 인벤토리가 눌리면 UI가 활성화되어야 한다.
        if( Input.GetKeyDown(KeyCode.R) )
        {
            playerController.inputLocked = !playerController.inputLocked;
            // 인벤토리가 열렸다면 화면을 초기화 해준다.
            if(playerController.inputLocked)
            {
                ItemCategory defaultCategory = (ItemCategory)(0);
                inventory_ui.ShowInventory(defaultCategory, seperatedItems[defaultCategory]);
            }

            inventory_ui.gameObject.SetActive(playerController.inputLocked);
        }


        // 인벤토리가 열려있다면
        if(inventory_ui.gameObject.activeSelf)
        {
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputY = Input.GetAxisRaw("Vertical");

            inventory_ui.MoveSelectItem((int)inputX, (int)inputY, seperatedItems[selectedCategory]);

            if( Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                int categoryCount = System.Enum.GetValues(typeof(ItemCategory)).Length;
                ItemCategory newCategory = (ItemCategory)Mathf.Clamp((int)selectedCategory - 1, 0, categoryCount - 1);

                if(newCategory != selectedCategory)
                {
                    selectedCategory = newCategory;
                    inventory_ui.ShowInventory(selectedCategory, seperatedItems[selectedCategory]);
                }
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                int categoryCount = System.Enum.GetValues(typeof(ItemCategory)).Length;
                ItemCategory newCategory = (ItemCategory)Mathf.Clamp((int)selectedCategory + 1, 0, categoryCount - 1);

                if (newCategory != selectedCategory)
                {
                    selectedCategory = newCategory;
                    inventory_ui.ShowInventory(selectedCategory, seperatedItems[selectedCategory]);
                }
            }

        }        
    }

    // todo 
    public void AquireItem(ItemSlot aquiredItem)
    {
        // 아이템 획득하면 아이템의 카데고리 정보를 읽어서
        // 딕셔너리에 추가해준다.
        var itemList = seperatedItems[aquiredItem.itemInformation.category];       
        for(int i = 0; i < itemList.Count; i++)
        {
            if( itemList[i].itemInformation.itemName == aquiredItem.itemInformation.itemName )
            {
                ItemSlot updatedSlot = itemList[i];
                updatedSlot.itemAmount += aquiredItem.itemAmount;

                itemList[i] = updatedSlot;
                return;
            }            
        }

        // new Item!!
        itemList.Add(aquiredItem);        
    }

}
