using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodCuttingControl : PlayerControlState
{
    [SerializeField] float startPosXOffset;
    [SerializeField] GameObject tool;

    float inputX;
    float progress = 0f;
    int progressDirection;
    [SerializeField] float progressMax = 10f;
    [SerializeField] float fillSpeed = 5f;

    WoodCollectable wood;

    public override void Enter()
    {
        wood = context.interactor.Interacted.GetComponent<WoodCollectable>();
        tool.SetActive(true);

        body.isKinematic = true;
        body.velocity = Vector3.zero; 

        var pos = body.position;
        pos.x = wood.transform.position.x + startPosXOffset;
        body.position = pos;

        animator.SetBool("Wood", true);

        progressDirection = 1;
    }

    public override void Exit()
    {
        tool.SetActive(false);
        body.isKinematic = false;

        animator.SetBool("Wood", false);
    }

    public override Mode GetMode()
    {
        return Mode.WoodCutting;
    }

    public override void NeedChagne()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || wood.isEndState())
        {
            context.ChangeState(Mode.Groud);
        }
    }

    public override void UpdateState()
    {
        float inputX = input.x;

        if((int)inputX == progressDirection)
        {
            progress += fillSpeed * Time.deltaTime;
        }

        body.position += Vector2.right * inputX * fillSpeed * Time.deltaTime;

        // 끝지점에 도달했다면 방향을 바꿔준다.
        if (progress >= progressMax)
        {
            wood.IncreamentStage();

            progressDirection = -progressDirection;
            progress = 0;
        }
    }
}
