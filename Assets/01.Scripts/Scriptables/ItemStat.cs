using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemCategory
{
    Wood, // 목재
    Ore, // 광석
    Fish, // 물고기
    Vegetable, // 야채
    Food, // 음식
    Ingot // 주괴 ( 광석 -> 주괴 )
}

public class ItemCategoryExtension
{
    public static string CategoryToString(ItemCategory category)
    {
        switch (category)
        {
            case ItemCategory.Wood:
                return "목재";
            case ItemCategory.Ore:
                return "광석";
            case ItemCategory.Fish:
                return "물고기";
            case ItemCategory.Vegetable:
                return "채소";
            case ItemCategory.Food:
                return "요리";
            case ItemCategory.Ingot:
                return "주괴";
            default:
                {
                    Debug.LogAssertion("아이템 카테고리 이름 초기화가 안되었습니다!!");
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
