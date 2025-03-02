using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderMovement : MovementState
{
    Detection detection;
    float distance = 1f;

    public LadderMovement(PlayerController context , Detection detection) : base(context)
    {
        this.detection = detection;
    }

    public override void End()
    {
        body.isKinematic = false;
        renderer.color = Color.white;

        animator.SetBool("ClimbLadder", false);

        body.velocity = new Vector2(input.x * moveSpeed, inputJump ? jumpSpeed : 0f);
    }

    public override bool NeedChagne(out MovementCategory category)
    {
        category = MovementCategory.Ladder;
        bool isClosedGround = false;
        // only check move direction and reached ground
        if (context.input.y < 0f)
        {
            float origin = collider.bounds.min.y;
            float to = detection.bound.min.y;

            distance = Mathf.Abs(origin - to);
            if (distance < 0.2f)
            {
                isClosedGround = true;
            }

        }
        else if (context.input.y > 0f)
        {
            float origin = collider.bounds.min.y;
            float to = detection.bound.max.y + 0.1f;

            distance = Mathf.Abs(origin - to);
            if (distance < 0.1f)
            {
                isClosedGround = true;
            }
        }

        if ((isClosedGround || !detection.isInner))
        {
            category = MovementCategory.Groud;
            return true;
        }

        if((context.inputJump && context.input_Abs.x > 0f))
        {
            category = MovementCategory.Jump;
            return true;
        }

        return false;
    }

    public override void Start()
    {
        distance = 1f;

        body.isKinematic = true;
        body.velocity = Vector2.zero;

        renderer.color = Color.yellow;


        float start_x = detection.bound.center.x;
        var startPos = body.position;
        startPos.x = detection.bound.center.x;
        body.position = startPos;

        animator.SetBool("ClimbLadder", true);

    }

    public override void Update()
    {
        Vector2 delta = Vector2.up * context.input.y * context.moveSpeed * Time.deltaTime;
        body.position = body.position + delta;
        animator.SetFloat("dir_y", context.input.y);
    }

    
}
