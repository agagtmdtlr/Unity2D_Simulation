using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Events;

public class UI_Cooking : MonoBehaviour
{
    [Header("UI Window")]
    public GameObject selectMaterialUI;
    public GameObject selectAmountUI;

    [Header("material container")]
    public GameObject materialView;
    public GameObject material_line_prefab;

    [Header("material description")]
    public Text material_name;
    public Text material_info;

    [Header("amount description")]
    public Image select_icn;
    public Text select_amount;

    public Button add_btn;
    public Button sub_bth;
    public Button cook_btn;
    
    ItemSlot current_material;
    int current_amount;

    public UnityEvent<ItemSlot, int> onClickCookButton;

    private void OnEnable()
    {
        cook_btn.onClick.RemoveAllListeners();
        cook_btn.onClick.AddListener(OnClickCookButton);

        add_btn.onClick.RemoveAllListeners();
        add_btn.onClick.AddListener(()=> { AddAmount(1); } );

        sub_bth.onClick.RemoveAllListeners();
        sub_bth.onClick.AddListener(()=> { AddAmount(-1); } );
    }

    void AddAmount(int amount)
    {
        current_amount += amount;
        current_amount = Mathf.Clamp(current_amount, 0, current_material.itemAmount);
        select_amount.text = current_amount.ToString();
    }

    void AddLine(int cnt)
    {
        for(int i = 0; i < cnt;i++)
        {
            Instantiate(material_line_prefab, materialView.transform);
        }
    }

    public void OnClickCookButton()
    {
        if(current_amount > 0)
            onClickCookButton.Invoke(current_material, current_amount);
    }

    public void OnClickMaterialInContainer(ItemSlot item)
    {
        material_name.text = item.itemInformation.itemName;
        material_info.text = item.itemInformation.description;

        select_icn.sprite = item.itemInformation.icon;
        current_material = item;
        current_amount = 0;
        select_amount.text = current_amount.ToString();
    }

   
    public void UpdateUI(ItemSlot[] items)
    {
        int columnSize = material_line_prefab.transform.childCount;
        int needLine = (items.Length - 1) / columnSize + 1;
        int curLine = materialView.transform.childCount;

        if( needLine > curLine)
        {
            int addcnt = needLine - curLine;
            AddLine(addcnt);
        }

        cook_btn.onClick.RemoveAllListeners();
        cook_btn.onClick.AddListener(OnClickCookButton);

        int maxItemCnt = needLine * columnSize;


        for ( int i = 0; i < maxItemCnt;i++)
        {
            int l = i / columnSize;
            int c = i % columnSize;

            if(i < items.Length)
            {
                var item = items[i];

                var itemUi = materialView.transform.GetChild(l).GetChild(c);
                itemUi.gameObject.SetActive(true);
                var icn = itemUi.GetComponent<Image>();
                icn.sprite = items[i].itemInformation.icon;
                itemUi.GetComponentInChildren<Text>().text = $"{item.itemAmount}";

                var btn = itemUi.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => { OnClickMaterialInContainer(item); });
            }
            else
            {
                var itemUi = materialView.transform.GetChild(l).GetChild(c);
                itemUi.gameObject.SetActive(false);
            }
        }
    }
}
