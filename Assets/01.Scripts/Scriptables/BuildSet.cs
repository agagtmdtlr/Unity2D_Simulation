using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildSet", menuName = "ScriptableObjects/BuildSet", order = 1)]
public class BuildSet : ScriptableObject
{
    public string buildname = string.Empty;
    public Sprite icn;
    public GameObject buildPrefab;
    [Header("건축재료")]
    public List<BuildMaterial> buldMaterial;
}

[System.Serializable]
public struct BuildMaterial
{
    public ItemStat itemStat;
    public int amount;
}

