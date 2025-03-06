using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonSpriteHandler : MonoBehaviour
{
    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite highlightSprite;

    Image image;
    private void Awake()
    {
        if( !TryGetComponent<Image>(out image) )
        {
            Debug.LogAssertion("�̹��� ������Ʈ�� �߰����ּ���");
        }

    }

    public void OnNormal()
    {
        image.sprite = normalSprite;
    }

    public void OnHightlight()
    {
        image.sprite = highlightSprite;

    }

}
