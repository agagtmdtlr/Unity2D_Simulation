using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public ItemSlot item;
    public SpriteRenderer spriteRenderer;
    Sensor interactable;

    private void Awake()
    {
        TryGetComponent(out spriteRenderer);
        TryGetComponent(out interactable);
        spriteRenderer.sprite = item.itemInformation.icon;
    }

    private void OnEnable()
    {
        interactable.interactEvent.AddListener( OnColleted);
    }

    private void OnDisable()
    {
        interactable.interactEvent.RemoveListener( OnColleted);
    }

    public void OnColleted(Sensor sensor)
    {
        if (interactable.interactor.TryGetComponent(out InventoryController inventory))
        {
            inventory.SetAmountOfItem(item.itemInformation, item.itemAmount);
            Destroy(gameObject);
        }
    }

}
