using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] ItemSlot item;
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        TryGetComponent(out spriteRenderer);
        spriteRenderer.sprite = item.itemInformation.icon;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if( collision.TryGetComponent(out InventoryController inventory) )
        {
            inventory.AquireItem(item);
            Destroy(gameObject);
        }

    }
}
