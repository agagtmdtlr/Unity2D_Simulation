using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleOutline : MonoBehaviour, IToggle
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] Color color = Color.red;
    [Range(0, 16)]
    [SerializeField] int outlineSize = 1;
    [SerializeField] Material outlineMaterial;

    public void Awake()
    {
        TryGetComponent(out spriteRenderer);
        spriteRenderer.material = outlineMaterial;
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
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", color);
        mpb.SetFloat("_OutlineSize", outlineSize);
        spriteRenderer.SetPropertyBlock(mpb);
    }
}

