using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{
    // Start is called before the first frame update
    InteractiveDetection detector;
    public Image targetUI;

    void Start()
    {
        detector = GetComponentInChildren<InteractiveDetection>();
    }

    // Update is called once per frame
    void Update()
    {
        // ��ȣ�ۿ� ����� ã�Ұ� , ��ȣ�ۿ� Ű�� �Է��Ͽ���.
        if(detector.detected && Input.GetKeyDown(KeyCode.E)) 
        {
            Debug.Log("Interaction with target");
        }
    }


    private void OnDrawGizmos()
    {
        if(detector && detector.detected )
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(detector.target.transform.position, 0.1f);

        }
    }
}
