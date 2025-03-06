using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    static private ItemSpawner instance = null;
    static public ItemSpawner Instance {  get { return instance;  } }

    private void Awake()
    {
        if(instance is null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] GameObject spawnPrefab;

    public void SpawnItem(Vector3 position, ItemStat spwanItem, int spawnCount)
    {
        GameObject spawnObject = Instantiate(spawnPrefab, position, Quaternion.identity);
        if (spawnObject.TryGetComponent(out Collectable collectable))
        {
            collectable.item.itemInformation = spwanItem;
            collectable.item.itemAmount = spawnCount;
        }
    }
}
