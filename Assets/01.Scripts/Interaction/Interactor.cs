using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    Collider2D bodyCollider;
    public float interactionRange = 1.0f;
    [SerializeField] private LayerMask whatIsInteractable;
    List<Interactable> founditeractables = new List<Interactable>();

    private void Awake()
    {
        TryGetComponent(out bodyCollider);
    }

    public Interactable GetInteractableObject()
    {
        founditeractables.Clear();

        {
            Collider2D[] colliders =
                Physics2D.OverlapCircleAll(
                    bodyCollider.bounds.center,
                    interactionRange,
                    whatIsInteractable);

            foreach (Collider2D other in colliders)
            {
                if (other.TryGetComponent(out Interactable interactable))
                {
                    interactable.OnStayInteractor();
                    founditeractables.Add(interactable);
                }
            }
        }


        Interactable target = null;
        float minDistance = Mathf.Infinity;
        foreach (Interactable other in founditeractables)
        {
            float dist = Vector3.Distance(bodyCollider.bounds.center, other.GetTransform().position);
            if (dist < minDistance)
            {
                minDistance = dist;
                target = other;
            }
        }

        return target;

    }

    void Update()
    {
        Interactable interactable = GetInteractableObject();
        // if interact clicked then find interactable objects
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(interactable != null)
            {
                interactable.Interact(gameObject);
            }
        }

    }

    private void OnDrawGizmos()
    {
        if (bodyCollider)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(bodyCollider.bounds.center, interactionRange);
        }
    }
}
