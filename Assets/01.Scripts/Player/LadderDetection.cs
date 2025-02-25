using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LadderDetection : MonoBehaviour
{
    [SerializeField] private bool debugDraw = true;
    public bool throughBound;
    public Bounds bound;
    private bool climbing;
    public Transform ladder;

    [SerializeField] Rigidbody2D body;
    [SerializeField] Vector2 offset;
    [SerializeField] Vector2 size;
    [SerializeField] public LayerMask ladderMask;

    private void Start()
    {
        body = GetComponentInParent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        /*throughLadder = false;
        Collider2D collision = Physics2D.OverlapBox(body.position + offset, size, ladderMask);

        if (collision != null)
        {
            ladderBound = collision.bounds;
            throughLadder = true;
        }*/

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        ladder = collision.transform;
        bound = collision.bounds;
        throughBound = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        throughBound = false;
    }
    private void OnGUI()
    {
        if(body)
            DebugTextManager.Write($"{body.position + offset}");

    }

    private void OnDrawGizmos()
    {
        if (!debugDraw)
            return;

        if (body)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(body.position + offset, size);
        }
            

    }



}
