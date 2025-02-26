using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemStat", menuName = "ScriptableObjects/ItemStat", order = 1)]
public class ItemStat : ScriptableObject
{
    public Sprite image;
    public string name;
    public int cost;
}
