using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundMovement : MovementState
{
    Detection climb;
    Detection ladder;

    public GroundMovement(
        PlayerController context,
        Detection climb,
        Detection ladder
        ) : base(context)
    {
        this.climb = climb;
        this.ladder = ladder;
    }

    public override void End()
    {
        context.isGrounded = false;
    }

    public override bool NeedChagne(out MovementCategory category)
    {
        category = MovementCategory.Groud;
        if(isGrounded == false)
        {
            category = MovementCategory.Jump;
            return true;
        }
        if(inputJump )
        {
            category = MovementCategory.Jump;
            return true;
        }
        if (climb.isInner && input.y > 0)
        {
            category = MovementCategory.Climb;
            return true;
        }
        if (ladder.isInner && Mathf.Abs(input.y) > 0.1f)
        {
            float distance;
            bool isCorretToBegin = true;
            if (input.y < 0f)
            {
                float origin = collider.bounds.min.y;
                float to = ladder.bound.min.y;

                distance = Mathf.Abs(origin - to);
                if (distance < 0.2f)
                {
                    isCorretToBegin = false;
                }

            }
            else if (input.y > 0f)
            {
                float origin = collider.bounds.min.y;
                float to = ladder.bound.max.y + 0.1f;

                distance = Mathf.Abs(origin - to);
                if (distance < 0.2f)
                {
                    isCorretToBegin = false;
                }
            }

            if(isCorretToBegin)
                category = MovementCategory.Ladder;
            return isCorretToBegin;
        }

        return false;
    }

    public override void Start()
    {
    }

    public override void Update()
    {
        Vector2 velocity = body.velocity;
        velocity.x = input.x * moveSpeed;

        body.velocity = !body.isKinematic ? velocity : Vector2.zero;

        if (Mathf.Abs(input.x) > 0)
        {
            renderer.flipX = input.x < 0;
        }
    }
}
