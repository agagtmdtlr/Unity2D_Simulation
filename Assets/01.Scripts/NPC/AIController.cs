using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private InteractiveHandler interactiveHandler;
    // UI
    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out interactiveHandler);
    }

    // Update is called once per frame
    void Update()
    {
        // UI 을 가져와서 
        
    }
}
