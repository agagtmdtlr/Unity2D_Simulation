using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveDetection : MonoBehaviour
{
    public bool detected;
    public GameObject target;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        detected = true;
        target = collision.transform.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        detected = false;
    }
}
