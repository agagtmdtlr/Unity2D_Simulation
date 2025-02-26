using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteractable : MonoBehaviour
{
    UI_NPC_Interaction npc_choice_ui;
    [SerializeField] GameObject bubble_chat_ui;
    Text bubble_chat_text;

    BoxCollider2D box;
    Interactable interaction;

    [SerializeField] Dialogue dialogue;
    int dialogueIndex = 0;

    private void Awake()
    {
        npc_choice_ui = FindAnyObjectByType<UI_NPC_Interaction>(FindObjectsInactive.Include);
        TryGetComponent(out box);
        TryGetComponent(out interaction);

        interaction.interactableUI.UI = npc_choice_ui.gameObject;
        bubble_chat_text = bubble_chat_ui.transform.GetComponentInChildren<Text>();

    }

    private void Update()
    {
        if(bubble_chat_text.text == dialogue.texts[dialogueIndex])
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                NextLine();
            }
        }
    }

    public void OpenNpcInteractSelector()
    {
        if( npc_choice_ui.transform.TryGetComponent(out RectTransform rt) )
        {
            rt.position = Camera.main.WorldToScreenPoint(box.bounds.center + Vector3.up * box.bounds.size.y);
        }

        interaction.interactableUI.UI = npc_choice_ui.gameObject;

        npc_choice_ui.BindSelectEvent(2, BeginDialogue);
        npc_choice_ui.gameObject.SetActive(true);
    }

    private void BeginDialogue()
    {
        dialogueIndex = 0;

        if (bubble_chat_ui.TryGetComponent(out RectTransform rt))
        {
            rt.position = Camera.main.WorldToScreenPoint(box.bounds.center + Vector3.up * box.bounds.size.y);
        }
        interaction.interactableUI.UI = bubble_chat_ui;
        bubble_chat_ui.gameObject.SetActive(true);

        StopCoroutine("Typing");
        StartCoroutine("Typing");
    }

    private void NextLine()
    {
        if( dialogueIndex + 1 < dialogue.texts.Length)
        {
            dialogueIndex++;
            StopCoroutine("Typing");
            StartCoroutine("Typing");
        }
    }

    private IEnumerator Typing()
    {

        WaitForSeconds wfs = new WaitForSeconds(0.1f);
        bubble_chat_text.text = "";


        foreach (char c in  dialogue.texts[dialogueIndex].ToCharArray())
        {
            bubble_chat_text.text += c;
            yield return wfs;
        }
    }

    private void OnEnable()
    {
        interaction.Interact.HasInteracted += OpenNpcInteractSelector;
    }

    private void OnDisable()
    {
        interaction.Interact.HasInteracted -= OpenNpcInteractSelector;
    }
}

