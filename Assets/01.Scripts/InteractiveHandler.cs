using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveHandler : MonoBehaviour
{
    [SerializeField] GameObject player;
    SpriteRenderer render;

    [SerializeField] Color InteractColor;

    SpriteOutline outline;

    public float distance;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindAnyObjectByType<PlayerController>().gameObject;
        TryGetComponent(out render);
        TryGetComponent(out outline);
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(player.transform.position, transform.position);
        outline.UpdateOutline(distance < 5.0f);

    }
}
