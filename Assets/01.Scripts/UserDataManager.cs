using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UserDataManager : Globalable<UserDataManager>
{
    [SerializeField] Button saveButton;
    [SerializeField] Sprite saveSprite;

    protected override void Awake_internal()
    {
        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(SaveUserData);
    }

    public void SaveUserData()
    {
        var inventory = FindAnyObjectByType<ItemStorage>();

        var itemsSeperated = inventory.SeperatedItems;
        foreach (var kvp in itemsSeperated)
        {
            foreach(var item in kvp.Value)
            {

            }
        }


        Debug.Log($"{saveSprite.ToString()} Complete Save User Data");

        Debug.Log($"{Application.persistentDataPath} Complete Save User Data");
    }
}
