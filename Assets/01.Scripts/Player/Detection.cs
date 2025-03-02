using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Detection : MonoBehaviour
{
    [SerializeField] public LayerMask whatIsDetectable;
    [SerializeField] public bool isInner { get; private set; }

    [HideInInspector] public Bounds bound;
    [HideInInspector] public Transform collisionTransform;

    private void Awake()
    {
        if( TryGetComponent(out Collider2D collider))
        {
            collider.callbackLayers |= whatIsDetectable;
        }
        else
        {
            Debug.LogAssertion("Detection을 수행하려면 반드신 Collider가 있어야 합니다");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        collisionTransform = collision.transform;
        bound = collision.bounds;
        isInner = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        isInner = false;
    }
}
