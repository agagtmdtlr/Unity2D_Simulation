using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbMovement : PlayerControlState
{
    [SerializeField] Detection detection;
    [SerializeField] float beginPosYOffset = 0.0f;
    [SerializeField] float endPosYOffset = 0.0f;

    public override Mode GetMode() { return Mode.Climb; }

    public override void Awake()
    {
        base.Awake();
    }

    public override void Exit()
    {
        // set climb end position with offset Y
        float platformY = detection.bound.max.y;
        Vector3 endPosition = 
            new Vector3(body.position.x, platformY + endPosYOffset, 0f);
        body.position = endPosition;

        animator.SetBool("ClimbPlatform", false);
        body.isKinematic = false;
    }

    public override void NeedChagne()
    {
        if (animator.GetBool("ClimbPlatform") == false)
        {
            context.ChangeState(Mode.Groud);
        }
    }

    public override void Enter()
    {
        body.isKinematic = true;
        velocity = Vector2.zero;

        // set climb start position with offset Y
        var startPosition = body.position;
        float platformY = detection.bound.max.y;
        platformY += beginPosYOffset;
        startPosition.y = platformY;
        body.position = startPosition;

        animator.SetBool("ClimbPlatform", true);

    }

    public override void UpdateState()
    {
        

    }

}
