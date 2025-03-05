using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    public GameObject ceilprefab;
    public GameObject floorprefab;

    private void Awake()
    {
        var box = GetComponent<BoxCollider2D>();
        float snappedSize = Mathf.Round(box.bounds.extents.y * 2f);
        float snappedExtend = snappedSize * 0.5f;
        var centerOffset = box.bounds.center - transform.position;

        {
            GameObject ceil = Instantiate(ceilprefab, transform);
            ceil.transform.localScale = Vector3.one;

            var cb = ceil.GetComponent<BoxCollider2D>();
            var sizeTo = (box.size);
            sizeTo.y = cb.size.y;
            cb.size = sizeTo;

            
            // bottom pivot에 맞추기 위해서 -0.5f
            var moveTo = (new Vector3( 0f, snappedExtend - 0.5f, 0));
            moveTo += centerOffset;
            ceil.transform.localPosition = moveTo;
        }
        {
            GameObject floor = Instantiate(floorprefab, transform);
            floor.transform.localScale = Vector3.one;

            var fb = floor.GetComponent<BoxCollider2D>();
            var sizeTo = (box.size);
            sizeTo.y = fb.size.y;
            fb.size = sizeTo;

            var moveTo = (new Vector3(0f, -snappedExtend - 0.5f, 0));
            moveTo += centerOffset;
            floor.transform.localPosition = moveTo;
        }

    }

    
}
