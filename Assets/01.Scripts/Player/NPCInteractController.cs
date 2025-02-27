using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class NPCInteractController : MonoBehaviour
{
    enum NPCInteractState
    {
        Choice,
        Gift,
        Food,
        Chat,
        Love,
        Info
    }

    NPCInteractState state;

    NPCInteractable interactable;

    // 상호작용 선택하는 UI
    UI_NPC_Interaction npc_choice_ui;

    // 선물 선택 UI

    // 요리 주기 UI

    // 대화 선택 후 대화창 UI
    UI_NPC_BubbleChat bubble_chat_ui;
    Text bubble_chat_text;

    // 교감 UI

    // 상태 확인 UI

    BoxCollider2D box;
    Dialogue dialogue;
    int dialogueIndex = 0;

    InventoryController inventoryHandler;
    Rigidbody2D body;

    float inputX, inputY;



    private void Awake()
    {
        interactable = null;
        npc_choice_ui = FindAnyObjectByType<UI_NPC_Interaction>(FindObjectsInactive.Include);
        bubble_chat_ui = FindAnyObjectByType<UI_NPC_BubbleChat>(FindObjectsInactive.Include);

        TryGetComponent(out inventoryHandler);
        TryGetComponent(out body);

    }

    private void Update()
    {
        if(interactable!= null)
        {
            if (isStopInteract())
            {
                EndNpcInteractMode();
            }
            else
            {
                switch (state)
                {
                    case NPCInteractState.Choice:
                        ChoiceMode();
                        break;
                    case NPCInteractState.Gift:
                        GiftMode();
                        break;
                    case NPCInteractState.Food:
                        FoodMode();
                        break;
                    case NPCInteractState.Chat:
                        ChatMode();
                        break;
                    case NPCInteractState.Love:
                        LoveMode();
                        break;
                    case NPCInteractState.Info:
                        InfoMode();
                        break;
                }
            }
        }
    }

    bool isStopInteract()
    {
        bool jump = Input.GetKeyDown(KeyCode.Space);
        bool escape = Input.GetKeyDown(KeyCode.Escape);
        return jump || escape;
    }

    public void StartNpcInteractMode(NPCInteractable interactable)
    {
        this.interactable = interactable;
        body.isKinematic = false;
        state = NPCInteractState.Choice;

    }

    public void EndNpcInteractMode()
    {
        this.interactable = null;
        body.isKinematic = true;
    }

    void ChoiceMode()
    {

    }

    void GiftMode()
    {
    }

    void FoodMode()
    {

    }

    void ChatMode()
    {

    }
    void LoveMode()
    {

    }

    void InfoMode()
    {

    }
}
