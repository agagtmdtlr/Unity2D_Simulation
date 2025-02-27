using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestController : MonoBehaviour
{
    PlayerController playerController;
    Animator animator;
    Rigidbody2D body;

    Harvestable harvestable;


    bool isHarvesting = false;

    float chargeGage = 0f;
    float chargeGageMax = 100f;
    float chargeSpeed = 5f;

    private void Awake()
    {
        TryGetComponent(out playerController);
        TryGetComponent(out body);
        TryGetComponent(out animator);
    }


    private void Update()
    {
        if(isHarvesting)
        {
            float inputX = Input.GetAxisRaw("Horizontal");
            animator.SetFloat("dir_x", inputX);

            Vector3 center = harvestable.transform.position;

            if (Input.GetKeyUp(KeyCode.E))
            {
                EndHarvestMode();
            }
        }
    }

    public void StartHarvestMode(Harvestable harvestable)
    {
        this.harvestable = harvestable;
        //playerController.inputLocked = true;
        body.isKinematic = true;
        isHarvesting = true;
        chargeGage = 0f;

        animator.SetBool("Harvest", true);
    }

    public void EndHarvestMode()
    {
        //playerController.inputLocked = false;
        body.isKinematic = false;
        isHarvesting = false;
        chargeGage = 0f;

        animator.SetBool("Harvest", false);

    }




}
