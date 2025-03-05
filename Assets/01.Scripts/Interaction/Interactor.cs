using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    Controllable control;
    Collider2D collider2d;
    public float interactionRange = 1.0f;
    [SerializeField] private LayerMask whatIsInteractable;
    List<Sensor> founditeractables = new List<Sensor>();

    Sensor closestInteractable;
    Sensor Interacted;

    public bool IsInteracting { get { return Interacted != null; } }

    private void Awake()
    {
        TryGetComponent(out collider2d);
        TryGetComponent(out control);
    }

    void EndInteracting()
    {
        Interacted.FocusOut();
        Interacted = null;
    }

    public Sensor GetInteractableObject()
    {
        founditeractables.Clear();

        {
            Collider2D[] colliders =
                Physics2D.OverlapCircleAll(
                    collider2d.bounds.center,
                    interactionRange,
                    whatIsInteractable);

            foreach (Collider2D other in colliders)
            {
                if(other.TryGetComponent(out Sensor interaciton))
                {
                    if(!interaciton.consumed)
                    {
                        if(interaciton.TriggerWay == InteractTriggerWay.Manual)
                            founditeractables.Add(interaciton);

                    }
                }    
            }
        }


        Sensor target = null;
        float minDistance = Mathf.Infinity;
        foreach (Sensor other in founditeractables)
        {
            float dist = Vector3.Distance(collider2d.bounds.center, other.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                target = other;
            }
        }

        return target;
    }

    private bool IsInnerRange(Vector3 position)
    {
        return Vector3.Distance(position, collider2d.bounds.center) <= interactionRange;
    }

    void Update()
    {
        Sensor interactable = GetInteractableObject();
        // 상화작용한 대상이 존재한다면? 범위를 벗어났다면 상호작용 안함
        if(Interacted != null && !IsInnerRange(Interacted.transform.position))
        {
            EndInteracting();
        }

        // 상호작용 중이 아니라면...
        if (Interacted == null && closestInteractable != interactable)
        {

            if (closestInteractable != null )
            {
                closestInteractable.FocusOut();
            }

            closestInteractable = interactable;
            if(closestInteractable != null)
            {
                closestInteractable.FocusIn();
            }
        }

        // if interact clicked then find interactable objects
        if (Interacted is null && control.InputInteract)
        {
            if(closestInteractable != null && Interacted != closestInteractable)
            {
                closestInteractable.CallInteract(this);
                Interacted = closestInteractable;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(collider2d)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(collider2d.bounds.center, interactionRange);
        }
    }
}
