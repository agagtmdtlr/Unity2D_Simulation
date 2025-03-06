using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CookRecipe", menuName = "ScriptableObjects/CookRecipe", order = 1)]

public class CookRecipe : ScriptableObject
{
    public ItemStat outitem;
    public int outAmount;
    public float cookTime;
    public ItemCategory[] categorys;
}
