using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_NPC_Interaction : MonoBehaviour
{
    [SerializeField] Image panel;

    UnityAction[] selectEvents;
    public int choiceCnt { get; set; } = 5;

    // Start is called before the first frame update
    void Start()
    {
        selectEvents = new UnityAction[choiceCnt];

        TryGetComponent(out panel);
        var childCnt = transform.childCount;

        var r = panel.rectTransform;
        var blockSize = r.rect.width / choiceCnt;
        var startX = (blockSize / 2) - (r.rect.width / 2);

        for(int i = 0; i < choiceCnt; i++)
        {
            var child = transform.GetChild(i);
            if( child.TryGetComponent(out RectTransform rt))
            {
                Vector3 p = Vector3.zero;
                p.x = startX + i * blockSize;
                rt.localPosition = p;
            }

            if( child.TryGetComponent(out Button btn))
            {

            }

        }
    }


    public void BindSelectEvent(int index, UnityAction action)
    {
        selectEvents[index] = action;
    }

    public void OnSelected(int index)
    {

        gameObject.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
