using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    bool isPlayer = false;
    Collider2D collider2d;
    PlatformEffector2D effector2d;
    private void Awake()
    {
        TryGetComponent(out effector2d);
        TryGetComponent(out collider2d);
    }

    private void Update()
    {
        if(isPlayer && Input.GetAxisRaw("Vertical") < 0f && Input.GetButtonDown("Jump"))
        {
            StartCoroutine(ReversePlatform_Co());
            isPlayer = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isPlayer = true;
        effector2d.surfaceArc = 180f;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isPlayer = false;
        effector2d.surfaceArc = 90f;
    }

    IEnumerator ReversePlatform_Co()
    {
        effector2d.rotationalOffset = 180f;
        collider2d.enabled = false;
        yield return new WaitForSeconds(0.5f);
        collider2d.enabled = true;

        effector2d.rotationalOffset = 0f;

    }
}
