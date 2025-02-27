using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemCategory
{
    Wood, // ����
    Ore, // ����
    Fish, // �����
    Vegetable, // ��ä
    Food, // ����
    Ingot // �ֱ� ( ���� -> �ֱ� )
}

public class ItemCategoryExtension
{
    public static string CategoryToString(ItemCategory category)
    {
        switch (category)
        {
            case ItemCategory.Wood:
                return "����";
            case ItemCategory.Ore:
                return "����";
            case ItemCategory.Fish:
                return "�����";
            case ItemCategory.Vegetable:
                return "ä��";
            case ItemCategory.Food:
                return "�丮";
            case ItemCategory.Ingot:
                return "�ֱ�";
            default:
                {
                    Debug.LogAssertion("������ ī�װ� �̸� �ʱ�ȭ�� �ȵǾ����ϴ�!!");
                    return "";
                }
        }
    }
}




[CreateAssetMenu(fileName = "ItemStat", menuName = "ScriptableObjects/ItemStat", order = 1)]
public class ItemStat : ScriptableObject
{
    public Sprite icon;
    public string itemName;
    public string description;
    public int cost;
    public ItemCategory category;
}
