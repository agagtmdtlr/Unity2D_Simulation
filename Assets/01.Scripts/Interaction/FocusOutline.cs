using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusOutline : MonoBehaviour, IFocusAction
{
    private SpriteRenderer spriteRenderer;
    MaterialPropertyBlock mpb;
    [SerializeField] Color color = Color.green;

    private void Awake()
    {
        TryGetComponent(out spriteRenderer);
        mpb = new MaterialPropertyBlock();
    }

    public void FocusOut()
    {
        UpdateOutline(false);
    }

    public void FocusIn()
    {
        UpdateOutline(true);
    }

    public void UpdateOutline(bool outline)
    {
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_OutlineVisible", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", color);
        //mpb.SetFloat("OutlineVisble", outlineSize);
        spriteRenderer.SetPropertyBlock(mpb);
    }
}

