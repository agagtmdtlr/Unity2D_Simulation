using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCollider : MonoBehaviour
{


    [SerializeField] private Color debugColor = Color.red;

    List<BoxCollider2D> box = new List<BoxCollider2D>();
    List<CircleCollider2D> circle = new List<CircleCollider2D>();
        

    // Start is called before the first frame update
    void Start()
    {
        GetComponents(box);
        GetComponents(circle);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = debugColor;
        if (box.Count > 0)
        {
            foreach(var b in box)
            {
                Gizmos.DrawWireCube(b.bounds.center, b.bounds.size);
            }

        }

        if (circle.Count > 0)
        {
            foreach (var c in circle)
            {
                Gizmos.DrawWireSphere(c.bounds.center, c.radius);
            }
        }
    }
}
