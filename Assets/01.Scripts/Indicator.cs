using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Indicator : MonoBehaviour
{
    [HideInInspector] public Transform target;
    public RectTransform ui;
    public RectTransform tail;
    public float radius = 1.0f;


    public void SetTarget(Transform target)
    { this.target = target; }

    private bool IsOffScreen()
    {
        Vector2 vec = Camera.main.WorldToViewportPoint(transform.position);
        if (vec.x < 0 || vec.x > 1 || vec.y <= 0 || vec.y > 1) return true;
        return false;
    }

    private void OnEnable()
    {
        ui = GetComponent<RectTransform>();
    }

    private void Update()
    {
        var position = Camera.main.WorldToScreenPoint(target.position);
       
        Vector2 min = new Vector2(ui.rect.width * 0.5f, ui.rect.height * 0.5f);
        Vector2 screenSize = Camera.main.ViewportToScreenPoint(Vector2.one);
        Vector2 max = new Vector2(screenSize.x - min.x, screenSize.y - min.y);

        position.x = Mathf.Clamp(position.x, min.x, screenSize.x - min.x);
        position.y = Mathf.Clamp(position.y, min.y, screenSize.y - min.y);

        Vector3 offset = Vector3.zero;
        if(IsOffScreen())
        {
            Vector2 dir = (target.position - Camera.main.ViewportToWorldPoint(Vector3.one * 0.5f) ).normalized;
            tail.right = dir;
            offset = dir * radius;
        }
        else
        {
            tail.right = Vector2.down;
            offset = Vector3.down * 0f;
        }
        ui.position = position;
        tail.position = position + offset;
    }
}
