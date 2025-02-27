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

public class InventoryHandler : MonoBehaviour
{
    [SerializeField] GameObject inventory;

    [Header("Inventory Category")]
    [SerializeField] GameObject category_container;
    [SerializeField] Text category_name_text;

    [Header("Inventory Own")]
    [SerializeField] Text own_cost_text;
    [SerializeField] GameObject item_container;
    [SerializeField] GameObject itemLine_prefab;

    [SerializeField] string item_bg_slot_path;
    [SerializeField] string itme_icn_slot_path;
    [SerializeField] string item_quantity_slot_path;

    [Header("Inventory Item Info")]
    [SerializeField] Text item_name_text;
    [SerializeField] Text item_cost_text;
    [SerializeField] Text item_description_text;

    [SerializeField] ItemSlot[] items;

    [SerializeField] Sprite selectedSprite;
    [SerializeField] Sprite defaultSprite;

    Rigidbody2D body;
    PlayerController playerController;
    int columnSize;

    GameObject selectedItemSlotUI;
    int selectedIndex;
    ItemCategory selectedCategory;

    Dictionary<ItemCategory, List<ItemSlot>> seperatedItems;

    private void Awake()
    {
        TryGetComponent(out body);
        TryGetComponent(out playerController);

        columnSize = itemLine_prefab.transform.childCount;
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


    

    void ItemSelected(int index)
    {
        var ItemList = seperatedItems[selectedCategory];
        ItemSlot item = ItemList[index];

        // 값을 입력 위치의 ui 객체를 찾는다.
        item_name_text.text = item.itemInformation.itemName;
        item_cost_text.text = item.itemInformation.cost.ToString();
        item_description_text.text = item.itemInformation.description;

        // 이전에 선택된 item ui가 있다면 배경 스프라이트 복구해준다.
        if(selectedItemSlotUI != null)
        {
            Image bgImage_before = GetItemBackground(selectedItemSlotUI);
            bgImage_before.sprite = defaultSprite;
        }

        GameObject itemSlotUI = FindItemSlotUI(index);
        Image bgImage= GetItemBackground(itemSlotUI);
        bgImage.sprite = selectedSprite;

        // 선택된 ui,index를 갱신해준다.
        selectedItemSlotUI = itemSlotUI;
        selectedIndex = index;
    }

    GameObject FindItemSlotUI(int itemIndex )
    {
        // 현재 들어갈 item의 위치를 계산한다.
        int x = itemIndex % columnSize;
        int y = itemIndex / columnSize;

        // 현재 들어갈 위치의 ui객체를 찾는다.
        Transform row = item_container.transform.GetChild(y);
        // 해당 라인이 꺼져있다면 켜주고 반환
        if(!row.gameObject.activeSelf)
        {
            row.gameObject.SetActive(true);
        }

        GameObject itemSlotUI = row.GetChild(x).gameObject;
        return itemSlotUI;
    }

    Image GetItemBackground(GameObject itemUI)
    {
        return itemUI.transform.Find(item_bg_slot_path).GetComponent<Image>();
    }


    Image GetItemIcon(GameObject itemUI)
    {
        return itemUI.transform.Find(itme_icn_slot_path).GetComponent<Image>();
    }




    void AddItem(ItemSlot item, int itemIndex)
    {

        // if nee new Line check new Line
        int curMaxSize = item_container.transform.childCount * columnSize;
        if( curMaxSize == 0 || itemIndex == curMaxSize - 1)
        {
            GameObject newItemLine = Instantiate(itemLine_prefab);
            newItemLine.transform.SetParent(item_container.transform);
            newItemLine.transform.localScale = Vector3.one;
        }

        GameObject itemSlotUI = FindItemSlotUI(itemIndex);
        itemSlotUI.SetActive(true);

        // 필요한 정보를 입력해준다.
        var imgObj = itemSlotUI.transform.Find(itme_icn_slot_path);
        Image imageSlot = imgObj.GetComponent<Image>();
        Button imageSlotButton = imgObj.GetComponent<Button>();

        imageSlotButton.onClick.RemoveAllListeners();
        imageSlotButton.onClick.AddListener(()=> { ItemSelected(itemIndex); });

        imageSlot.sprite = item.itemInformation.icon;

        Text quantitySlot = itemSlotUI.transform.Find(item_quantity_slot_path).GetComponent<Text>();
        quantitySlot.text = item.itemAmount.ToString();
    }

    void ClearInventory()
    {
        selectedItemSlotUI = null;

        int rowSize = item_container.transform.childCount;
        for(int i = 0; i < rowSize;i++)
        {
            var row =  item_container.transform.GetChild(i);
            int colSize = row.childCount;
            for(int c = 0; c < colSize; c++)
            {
                var item = row.GetChild(c).gameObject;
                var bg = GetItemBackground(item);
                bg.sprite = defaultSprite;

                item.SetActive(false);
            }
            row.gameObject.SetActive(false);
        }

        // 값을 입력 위치의 ui 객체를 찾는다.
        item_name_text.text = "";
        item_cost_text.text = "";
        item_description_text.text = "";

        // 아이템 정보를 입력하기 전에
        // 화면 정보를 정리해준다..
        // item slot 객체들을 전부 비활성화 해준다 . (삭제 X)
        // todo unused ui 로 객체들을 옮겨 보과해두는 방법도 생각해야 한다.
    }
    

    void ShowInventory(ItemCategory category)
    {
        category_name_text.text = ItemCategoryExtension.CategoryToString(category);
        selectedCategory = category;

        ClearInventory();
        var itemList = seperatedItems[category];
        int itemCnt = itemList.Count;
        for (int i = 0; i < itemCnt; i++)
        {
            AddItem(itemList[i], i);
        }
        if (itemList.Count > 0)
            ItemSelected(0);
    }

    void MoveSelectItem(int inputX, int inputY)
    {
        var itemList = seperatedItems[selectedCategory];

        // 이동이 없다면 종료
        if (itemList.Count == 0)
        {
            return;
        }

        int curIndex = selectedIndex;
        curIndex += inputX;
        curIndex += inputY * columnSize;
        curIndex = Mathf.Clamp(curIndex, 0, itemList.Count - 1);

        if(selectedIndex == curIndex)
        {
            return;
        }

        ItemSelected(curIndex);
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
                ShowInventory(defaultCategory);
            }

            inventory.SetActive(playerController.inputLocked);
        }


        // 인벤토리가 열려있다면
        if(inventory.activeSelf)
        {
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputY = Input.GetAxisRaw("Vertical");

            MoveSelectItem((int)inputX, (int)inputY);

            if( Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                int categoryCount = System.Enum.GetValues(typeof(ItemCategory)).Length;
                ItemCategory newCategory = (ItemCategory)Mathf.Clamp((int)selectedCategory - 1, 0, categoryCount - 1);

                if(newCategory != selectedCategory)
                {
                    ShowInventory(newCategory);
                }
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                int categoryCount = System.Enum.GetValues(typeof(ItemCategory)).Length;
                ItemCategory newCategory = (ItemCategory)Mathf.Clamp((int)selectedCategory + 1, 0, categoryCount - 1);

                if (newCategory != selectedCategory)
                {
                    ShowInventory(newCategory);
                }
            }


            // 키보드 방향에 맞춰 아이템을 선택한다.
            int row_count = item_container.transform.childCount;
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
