using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteractable : InteractableObject
{
    UI_NPC_Interaction npc_choice_ui;
    BoxCollider2D box;
    List<Button> buttons = new List<Button>(1);

    protected override void Start()
    {
        base.Start();
        npc_choice_ui = FindAnyObjectByType<UI_NPC_Interaction>();
        TryGetComponent(out box);
    }

    protected override void Update()
    {
        base.Update();        
    }

    protected override void OnStayInteractor()
    {

    }

    protected override void OnExitInteractor()
    {
        npc_choice_ui.gameObject.SetActive(false);
    }

    protected override void OnInteract(GameObject executer)
    {
        if( npc_choice_ui.transform.TryGetComponent(out RectTransform rt) )
        {
            rt.position = Camera.main.WorldToScreenPoint(box.bounds.center + Vector3.up * box.bounds.size.y);
        }
        npc_choice_ui.gameObject.SetActive(true);
    }
}
