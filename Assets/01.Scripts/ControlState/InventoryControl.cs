using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryControl : PlayerControlState
{
    ItemStorage storage;
    ItemCategory selectedCategory;
    Dictionary<ItemCategory, List<ItemSlot>> SeperatedItems { get { return storage.SeperatedItems; } }

    UI_Inventory ui { get { return storage.ui; } }    

    public override Mode GetMode() { return Mode.Inventory; }

    public override void Awake()
    {
        base.Awake();
        this.storage = context.GetComponent<ItemStorage>();
    }

    public override void Enter()
    {
        ui.gameObject.SetActive(true);
        ItemCategory defaultCategory = (ItemCategory)(0);
        ui.ShowInventory(defaultCategory, storage.SeperatedItems[defaultCategory]);
    }

    public override void Exit()
    {
        ui.gameObject.SetActive(false);
    }

    public override void UpdateState()
    {
        // 인벤토리가 열려있다면
        if (ui.gameObject.activeSelf)
        {

            ui.MoveSelectItem((int)input.x, (int)input.y, SeperatedItems[selectedCategory]);

            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                int categoryCount = System.Enum.GetValues(typeof(ItemCategory)).Length;
                ItemCategory newCategory = (ItemCategory)Mathf.Clamp((int)selectedCategory - 1, 0, categoryCount - 1);

                if (newCategory != selectedCategory)
                {
                    selectedCategory = newCategory;
                    ui.ShowInventory(selectedCategory, SeperatedItems[selectedCategory]);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                int categoryCount = System.Enum.GetValues(typeof(ItemCategory)).Length;
                ItemCategory newCategory = (ItemCategory)Mathf.Clamp((int)selectedCategory + 1, 0, categoryCount - 1);

                if (newCategory != selectedCategory)
                {
                    selectedCategory = newCategory;
                    ui.ShowInventory(selectedCategory, SeperatedItems[selectedCategory]);
                }
            }

        }
    }


    public override void NeedChagne()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            context.ChangeState(Mode.Groud);
        }
    }
}

