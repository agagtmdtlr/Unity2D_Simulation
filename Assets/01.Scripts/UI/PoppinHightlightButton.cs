using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PoppinHightlightButton : Button
{
    Coroutine coroutine;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (coroutine != null)
        {
            return;
        }
        StartCoroutine(PointerEnter_Co(GetComponent<RectTransform>()));
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }

    private IEnumerator PointerEnter_Co(RectTransform transform)
    {
        WaitForSeconds wfs = new WaitForSeconds(0.1f);
        float defaultW = transform.rect.width;

        float t = 0f;
        while(t < 1f)
        {
            float curVal = EasingFunction.EaseInSine(1f, 2f, t);
            transform.localScale = new Vector2(curVal, curVal);
            yield return wfs;
            t += 0.1f;
        }
        transform.localScale = Vector2.one;
        yield break;
    }
}
