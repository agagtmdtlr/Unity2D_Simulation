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
            Debug.LogAssertion("이미지 컴포넌트를 추가해주세요");
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
