using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingHandler : MonoBehaviour
{
    [Header("UI Reference")]
    public UI_Cooking ui;

    [Header("Recipe For Cooking")]
    public CookRecipe[] recipes;

    [Header("Cooking Effect")]
    public Transform clock;

    Sensor sensor;
    bool interacting;
    public bool Interacting { get { return interacting; } }


    bool isCooking;
    public bool IsCooking { get { return isCooking; } }
    CookRecipe cookingRecipe;
    int cookingAmount;
    float cookingTime;
    float endCookingTime;

    List<CookRecipe>[] seperatedRecipeContainer; 

    private void Awake()
    {
        ui = FindAnyObjectByType<UI_Cooking>(FindObjectsInactive.Include);

        TryGetComponent(out sensor);

        int categoryCount = System.Enum.GetValues(typeof(ItemCategory)).Length;
        seperatedRecipeContainer = new List<CookRecipe>[categoryCount];

        for (int i = 0; i < categoryCount;i++)
        {
            seperatedRecipeContainer[i] = new List<CookRecipe>();
        }

        foreach(var recipe in recipes)
        {
            foreach(var category in recipe.categorys)
            {
                seperatedRecipeContainer[(int)category].Add(recipe);
            }
        }
    }

    private void OnEnable()
    {
        sensor.interactEvent.AddListener(BeginCookInteraction);
    }

    private void OnDisable()
    {
        sensor.interactEvent.RemoveListener(BeginCookInteraction);
    }

    private void Update()
    {
        if(interacting)
        {
            if(Input.GetKeyDown(KeyCode.Escape) )
            {
                EndCookInteraction();
            }
        }
        else if(isCooking)
        {
            cookingTime += Time.deltaTime;

            float zRotation = Mathf.Lerp(0, 360, Mathf.Clamp(cookingTime / endCookingTime, 0, 1));
            Vector3 rotation = new Vector3(0, 0, zRotation);
            clock.rotation = Quaternion.Euler(rotation);
        }
    }

    private void BeginCookInteraction(Sensor sensor)
    {
        clock.rotation = Quaternion.identity;
        var interactor = sensor.Interactor;
        if (isCooking)
        {
            // 요리된 음식을 반환한다.
            int spawnAmount = cookingRecipe.outAmount * cookingAmount;
            ItemSpawner.Instance.SpawnItem(transform.position, cookingRecipe.outitem, spawnAmount);
            isCooking = false;
            EndCookInteraction();
        }
        else
        {
            interacting = true;

            ui.gameObject.SetActive(true);

            ItemStorage inventory = sensor.Interactor.GetComponent<ItemStorage>();
            ui.UpdateUI(inventory.SeperatedItems[ItemCategory.Fish]);

            ui.onClickCookButton.AddListener(StartCooking);
        }


    }
    
    private void StartCooking(ItemSlot item, int amount)
    {
        if( FindValidRecipe(item, out CookRecipe recipe) )
        {
            isCooking = true;
            cookingTime = 0;
            endCookingTime = amount * recipe.cookTime;
            cookingAmount = amount;
            cookingRecipe = recipe;
        }

        EndCookInteraction();
    }

    bool FindValidRecipe(ItemSlot item, out CookRecipe recipe)
    {
        var list = seperatedRecipeContainer[(int)item.itemInformation.category];
        if(list.Count > 0)
        {
            recipe = list[0];
            return true;
        }
        recipe = null;
        return false;
    }

    private void EndCookInteraction()
    {
        interacting = false;

        ui.gameObject.SetActive(false);

        var interactor = sensor.Interactor;
        if (interactor.TryGetComponent(out Controllable control))
        {
            control.UnLock(this);
        }
    }
}



