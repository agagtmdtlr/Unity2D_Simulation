using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusBubble : MonoBehaviour, IFocusAction
{
    GameObject bubble;
    RectTransform bubbleTransform;
    Collider2D collider;

    private void Start()
    {
        bubble = UISpawner.Instance.SpawnInteractionBubbleUI();
        bubble.SetActive(false);

        TryGetComponent(out collider);
        bubble.TryGetComponent(out bubbleTransform);
    }

    private void Update()
    {
        bubbleTransform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * collider.bounds.extents.y);
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
}
