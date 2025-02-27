using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_NPC_Interaction : MonoBehaviour
{
    RectTransform rt;
    private UnityAction[] selectEvents;
    public int choiceCnt { get; private set; }
    public Vector3 showWorldPosiiton;

    // Start is called before the first frame update
    void Awake()
    {
        choiceCnt = transform.childCount;
        selectEvents = new UnityAction[choiceCnt];
        TryGetComponent(out RectTransform rt);
    }

    private void OnEnable()
    {
        rt.position = Camera.main.WorldToScreenPoint(showWorldPosiiton);
    }

    private void Update()
    {
        if( Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
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

}
