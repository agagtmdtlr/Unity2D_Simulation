using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public ItemSlot item;
    public SpriteRenderer spriteRenderer;
    Interactable interactable;

    private void Awake()
    {
        TryGetComponent(out spriteRenderer);
        TryGetComponent(out interactable);
        spriteRenderer.sprite = item.itemInformation.icon;
    }

    public void OnColleted()
    {
        if (interactable.interactor.TryGetComponent(out InventoryController inventory))
        {
            inventory.AquireItem(item);
            Destroy(gameObject);
        }
    }

}
