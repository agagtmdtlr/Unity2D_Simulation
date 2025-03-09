using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public Controllable control;
    CircleCollider2D circle;

    public List<Sensor> innerSensors = new List<Sensor>();
    public Sensor Interacted { get; private set; }

    public bool IsInteracting { get { return Interacted != null; } }

    public void AddSenser(Sensor sensor)
    {
        innerSensors.Add(sensor);
    }

    public void RemoveSensor(Sensor sensor)
    {
        if( Interacted == sensor)
        {
            Interacted = null;
        }
        innerSensors.Remove(sensor);
    }

    private void Awake()
    {
        TryGetComponent(out circle);
    }

    public Sensor DoInteract()
    {
        Interacted = GetInteractableObject();
        if(Interacted != null)
            Interacted.CallInteract(this);
        return Interacted;
    }

public Sensor GetInteractableObject()
    {
        Sensor target = null;
        float minDistance = Mathf.Infinity;
        foreach (Sensor other in innerSensors)
        {
            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                target = other;
            }
        }
        return target;
    }

    private void OnDrawGizmos()
    {
        if(circle)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(circle.bounds.center, circle.radius);
        }
    }
}
