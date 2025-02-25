using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbDetection : MonoBehaviour
{
    [SerializeField] private bool climbPlatform;
    public bool throungBound;
    public Bounds bound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        throungBound = true;
        if (collision.transform.parent.TryGetComponent(out Collider2D platformColl))
        {
            bound = platformColl.bounds;
        }
        else
        {
            Debug.LogAssertion("failed get parent collider");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        throungBound = false;
    }

}
