using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderMovement : MonoBehaviour
{
    [Header("Ladder Info")]
    private bool climbing;
    [SerializeField] LadderDetection ladderdection;

    void Start()
    {
        ladderdection = GetComponentInChildren<LadderDetection>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
