using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AIInteractable : MonoBehaviour
{
    AIController aiController;
    UI_NPC_Interaction ui;
    [SerializeField] GameObject bubble_chat_ui;
    Text bubble_chat_text;

    BoxCollider2D box;

    [SerializeField] Dialogue dialogue;
    int dialogueIndex = 0;

    private void Start()
    {
        ui = FindAnyObjectByType<UI_NPC_Interaction>(FindObjectsInactive.Include);
        TryGetComponent(out aiController);
        TryGetComponent(out box);

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

    public void BeginAiInteraction(Sensor sensor)
    {
    }

    private void BeginDialogue()
    {
        dialogueIndex = 0;

        if (bubble_chat_ui.TryGetComponent(out RectTransform rt))
        {
            rt.position = Camera.main.WorldToScreenPoint(box.bounds.center + Vector3.up * box.bounds.size.y);
        }
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
}

