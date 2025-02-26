using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHandler : MonoBehaviour
{
    [SerializeField] ItemStat[] items;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // 인벤토리가 눌리면 UI가 활성화되어야 한다.
        if( Input.GetKeyDown(KeyCode.R))
        {

        }
        
    }
}
