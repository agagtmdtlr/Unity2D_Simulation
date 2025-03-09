using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
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

    GameObject selectedItemSlotUI;
    int selectedIndex;

    Image GetItemBackground(GameObject itemUI)
    {
        return itemUI.transform.Find(item_bg_slot_path).GetComponent<Image>();
    }

    GameObject FindItemSlotUI(int itemIndex)
    {
        int columnSize = item_container.transform.childCount;

        // ���� �� item�� ��ġ�� ����Ѵ�.
        int x = itemIndex % columnSize;
        int y = itemIndex / columnSize;

        // ���� �� ��ġ�� ui��ü�� ã�´�.
        Transform row = item_container.transform.GetChild(y);
        // �ش� ������ �����ִٸ� ���ְ� ��ȯ
        if (!row.gameObject.activeSelf)
        {
            row.gameObject.SetActive(true);
        }

        GameObject itemSlotUI = row.GetChild(x).gameObject;
        return itemSlotUI;
    }

    void ItemSelected(int index,ItemSlot item)
    {
        // ���� �Է� ��ġ�� ui ��ü�� ã�´�.
        item_name_text.text = item.itemInformation.itemName;
        item_cost_text.text = item.itemInformation.cost.ToString();
        item_description_text.text = item.itemInformation.description;

        // ������ ���õ� item ui�� �ִٸ� ��� ��������Ʈ �������ش�.
        if (selectedItemSlotUI != null)
        {
            Image bgImage_before = GetItemBackground(selectedItemSlotUI);
            bgImage_before.sprite = defaultSprite;
        }

        GameObject itemSlotUI = FindItemSlotUI(index);
        Image bgImage = GetItemBackground(itemSlotUI);
        bgImage.sprite = selectedSprite;

        // ���õ� ui,index�� �������ش�.
        selectedItemSlotUI = itemSlotUI;
        selectedIndex = index;
    }

    void AddItem(ItemSlot item, int itemIndex)
    {
        int columnSize = item_container.transform.childCount;
        // if nee new Line check new Line
        int curMaxSize = item_container.transform.childCount * columnSize;
        if (curMaxSize == 0 || itemIndex == curMaxSize - 1)
        {
            GameObject newItemLine = Instantiate(itemLine_prefab);
            newItemLine.transform.SetParent(item_container.transform);
            newItemLine.transform.localScale = Vector3.one;
        }

        GameObject itemSlotUI = FindItemSlotUI(itemIndex);
        itemSlotUI.SetActive(true);

        // �ʿ��� ������ �Է����ش�.
        var imgObj = itemSlotUI.transform.Find(itme_icn_slot_path);
        Image imageSlot = imgObj.GetComponent<Image>();
        Button imageSlotButton = imgObj.GetComponent<Button>();

        imageSlotButton.onClick.RemoveAllListeners();
        imageSlotButton.onClick.AddListener(() => { ItemSelected(itemIndex, item); });

        imageSlot.sprite = item.itemInformation.icon;

        Text quantitySlot = itemSlotUI.transform.Find(item_quantity_slot_path).GetComponent<Text>();
        quantitySlot.text = item.itemAmount.ToString();
    }

    void ClearInventory()
    {
        // ������ ������ �Է��ϱ� ����
        // ȭ�� ������ �������ش�..
        // item slot ��ü���� ���� ��Ȱ��ȭ ���ش� . (���� X)
        // todo unused ui �� ��ü���� �Ű� �����صδ� ����� �����ؾ� �Ѵ�.
        selectedItemSlotUI = null;

        int rowSize = item_container.transform.childCount;
        for (int i = 0; i < rowSize; i++)
        {
            var row = item_container.transform.GetChild(i);
            int colSize = row.childCount;
            for (int c = 0; c < colSize; c++)
            {
                var item = row.GetChild(c).gameObject;
                var bg = GetItemBackground(item);
                bg.sprite = defaultSprite;

                item.SetActive(false);
            }
            row.gameObject.SetActive(false);
        }

        // ���� �Է� ��ġ�� ui ��ü�� ã�´�.
        item_name_text.text = "";
        item_cost_text.text = "";
        item_description_text.text = "";
    }

    public void ShowInventory(ItemCategory category, List<ItemSlot> itemList)
    {
        ClearInventory();

        category_name_text.text = ItemCategoryExtension.CategoryToString(category);
        int itemCnt = itemList.Count;
        for (int i = 0; i < itemCnt; i++)
        {
            AddItem(itemList[i], i);
        }
        if (itemList.Count > 0)
            ItemSelected(0, itemList[0]);
    }

    public void MoveSelectItem(int inputX, int inputY, List<ItemSlot> itemList)
    {
        if (itemList.Count == 0)
        {
            return;
        }

        int columnSize = item_container.transform.childCount;
        int curIndex = selectedIndex;
        curIndex += inputX;
        curIndex += inputY * columnSize;
        curIndex = Mathf.Clamp(curIndex, 0, itemList.Count - 1);

        if (selectedIndex == curIndex)
        {
            return;
        }

        ItemSelected(curIndex, itemList[curIndex]);
    }

}
