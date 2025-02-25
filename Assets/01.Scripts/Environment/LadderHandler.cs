using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderHandler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out renderer);

        Transform top = transform.GetChild(0);
        Transform bottom = transform.GetChild(1);

        top.position = new Vector2( transform.position.x , transform.position.y + renderer.size.y);
        bottom.position = new Vector2( transform.position.x , transform.position.y);

    }

    // Update is called once per frame
    void Update()
    {        
    }

    private void OnDrawGizmos()
    {
        
    }
}
