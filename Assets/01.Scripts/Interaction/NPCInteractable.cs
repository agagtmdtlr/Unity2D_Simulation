using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteractable : Interactable
{
    UI_NPC_Interaction npc_choice_ui;
    BoxCollider2D box;
    List<Button> buttons = new List<Button>(1);
    Interactor interactor;
    protected override void Start()
    {
        base.Start();
        npc_choice_ui = FindAnyObjectByType<UI_NPC_Interaction>();
        TryGetComponent(out box);
    }

    protected override void Update()
    {
        base.Update();     
        
        if(interactor)
        {
            float dist = Vector3.Distance(interactor.transform.position, transform.position);
            if( dist > interactor.interactionRange )
            {
                npc_choice_ui.gameObject.SetActive(false);
                interactor = null;  
            }
         
        }    
    }

    public override void Interact(GameObject executer)
    {
        executer.TryGetComponent(out interactor);

        if( npc_choice_ui.transform.TryGetComponent(out RectTransform rt) )
        {
            rt.position = Camera.main.WorldToScreenPoint(box.bounds.center + Vector3.up * box.bounds.size.y);
        }
        npc_choice_ui.gameObject.SetActive(true);
    }
}
