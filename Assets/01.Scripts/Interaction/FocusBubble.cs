using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FocusBubble : FocusAction
{
    GameObject bubble;
    RectTransform ui;
    Collider2D collider2d;

    private void Start()
    {
        bubble = UISpawner.Instance.SpawnInteractionBubbleUI();
        bubble.SetActive(false);

        TryGetComponent(out collider2d);
        bubble.TryGetComponent(out ui);
    }

    private void Update()
    {
        ui.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * collider2d.bounds.extents.y);
    }

    private void OnDisable()
    {
        Destroy(bubble);
    }

    public void FocusOut()
    {
        bubble.SetActive(false);
    }

    public void FocusIn()
    {
        bubble.SetActive(true);

        bubble.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * GetComponent<Collider2D>().bounds.extents.y);
    }

    public override void FocusIn(Transform toFocus)
    {
        bubble = UISpawner.Instance.SpawnInteractionBubbleUI();
        bubble.SetActive(true);
    }

    public override void FocusOut(Transform toFocus)
    {
        Object.Destroy(bubble.gameObject);
    }
}
