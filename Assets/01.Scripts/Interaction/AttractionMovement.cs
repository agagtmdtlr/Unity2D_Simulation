using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractionMovement : MonoBehaviour
{
    [SerializeField] float attractionRange;
    [SerializeField] float attractionSpeed;
    [SerializeField] LayerMask whatIsAttractable;

    void Update()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, attractionRange, whatIsAttractable);
        if(collider != null)
        {
            Vector3 dir = collider.transform.position - transform.position;
            dir = dir.normalized;

            transform.position += dir * attractionSpeed * Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attractionRange);
    }
}
