using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_NPC_Interaction : MonoBehaviour
{
    [SerializeField] Image panel;

    UnityAction[] selectEvents = new UnityAction[choiceCnt];
    public const int choiceCnt = 5;

    // Start is called before the first frame update
    void Awake()
    {
        TryGetComponent(out panel);
        var childCnt = transform.childCount;
        for(int i = 0; i < choiceCnt; i++)
        {

        }
    }


    public void BindSelectEvent(int index, UnityAction action)
    {
        selectEvents[index] = action;
    }

    public void OnSelected(int index)
    {
        selectEvents[index]?.Invoke();
        gameObject.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
