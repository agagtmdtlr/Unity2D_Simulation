using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MineCollectable : MonoBehaviour
{
    [SerializeField] float defaultHP = 2f;
    [SerializeField] float hp = 2f;
    public float HP { get { return hp; } }

    Collider2D collider2d;


    private void Awake()
    {
        TryGetComponent(out collider2d);
    }

    private void OnEnable()
    {
        hp = defaultHP;
        collider2d.enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            if (TryGetComponent(out Spawnable spawnable))
            {
                spawnable.Spawn(transform.position);
            }

            GetComponent<SpriteRenderer>().enabled = false;
            collider2d.enabled = false;
        }
    }

}
