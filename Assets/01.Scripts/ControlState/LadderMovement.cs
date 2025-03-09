using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderMovement : PlayerControlState
{
    [SerializeField] float moveSpeed;
    [SerializeField] Detection detection;
    float distance = 1f;

    public override Mode GetMode() { return Mode.Ladder; }

    public override void Awake()
    {
        base.Awake();
    }

    public override void Exit()
    {
        body.isKinematic = false;
        renderer2d.color = Color.white;

        animator.SetBool("ClimbLadder", false);

        body.velocity = new Vector2(input.x * moveSpeed, inputJump ? moveSpeed : 0f);
    }

    public override void NeedChagne()
    {
        bool isClosedGround = false;
        // only check move direction and reached ground
        if (input.y < 0f)
        {
            float origin = collider2d.bounds.min.y;
            float to = detection.bound.min.y;

            distance = Mathf.Abs(origin - to);
            if (distance < 0.2f)
            {
                isClosedGround = true;
            }

        }
        else if (input.y > 0f)
        {
            float origin = collider2d.bounds.min.y;
            float to = detection.bound.max.y + 0.1f;

            distance = Mathf.Abs(origin - to);
            if (distance < 0.1f)
            {
                isClosedGround = true;
            }
        }

        if ((isClosedGround || !detection.isInner))
        {
            context.ChangeState(Mode.Groud);
            return;
        }
        
        if((inputJump && input_Abs.x > 0f))
        {
            context.ChangeState(Mode.Jump);
            return;
        }

    }

    public override void Enter()
    {
        distance = 1f;

        body.isKinematic = true;
        body.velocity = Vector2.zero;

        renderer2d.color = Color.yellow;


        float start_x = detection.bound.center.x;
        var startPos = body.position;
        startPos.x = detection.bound.center.x;
        body.position = startPos;

        animator.SetBool("ClimbLadder", true);

    }

    public override void UpdateState()
    {
        Vector2 delta = Vector2.up * input.y * moveSpeed * Time.deltaTime;
        body.position = body.position + delta;
        animator.SetFloat("dir_y", input.y);
    }

    
}
