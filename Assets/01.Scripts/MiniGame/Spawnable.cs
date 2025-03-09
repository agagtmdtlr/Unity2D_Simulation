using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnable : MonoBehaviour
{
    [SerializeField] ItemStat spwanItem;
    [SerializeField] int spawnCount;

    public void Spawn(Vector3 position)
    {
        ItemSpawner.Instance.SpawnItem(position, spwanItem, spawnCount);
    }

    public ItemCategory GetCategory()
    {
        return spwanItem.category;
    }
}
