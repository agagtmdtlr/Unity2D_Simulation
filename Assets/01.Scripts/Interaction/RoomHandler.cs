using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomHandler : MonoBehaviour
{
    SpriteRenderer doorRender;

    bool inside = false;

    public SpriteRenderer[] hideWhenDoorOpen;
    public GameObject[] activateWhenInside;


    private void Awake()
    {
        TryGetComponent(out doorRender);
    }

    private void OnEnable()
    {
        GetComponent<Sensor>().interactEvent.AddListener(DoorOpen);
    }

    private void OnDisable()
    {
        GetComponent<Sensor>().interactEvent.RemoveListener(DoorOpen);
    }

    private void DoorOpen(Sensor sensor)
    {
        inside = !inside;
        var doorColor = doorRender.color;
        doorColor.a = inside ? 0.5f : 1f;
        doorRender.color = doorColor;
        foreach (var r in hideWhenDoorOpen)
        {
            var color = r.color;
            color.a = inside ? 0f : 1f;
            r.color = color;
        }
        foreach (var obj in activateWhenInside)
        {
            obj.SetActive(inside);
        }
    }
}
