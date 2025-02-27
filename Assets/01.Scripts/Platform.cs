using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    bool isPlayer = false;
    PlatformEffector2D effector2d;
    private void Awake()
    {
        TryGetComponent(out effector2d);
    }

    private void Update()
    {
        if(isPlayer && Input.GetAxisRaw("Vertical") < 0f)
        {
            StartCoroutine(ReversePlatform_Co());
            isPlayer = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isPlayer = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isPlayer = false;
    }

    IEnumerator ReversePlatform_Co()
    {
        effector2d.rotationalOffset = 180f;

        yield return new WaitForSeconds(0.5f);

        effector2d.rotationalOffset = 0f;

    }
}
