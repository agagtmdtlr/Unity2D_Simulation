using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpMovement : MovementState
{
    Detection climb;
    Detection ladder;

    public JumpMovement(
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
        //context.isGrounded = true;
    }

    public override bool NeedChagne(out MovementMode category)
    {
        if(Mathf.Abs(velocity.y) < 0.01f)
        {
            category = MovementMode.Groud;
            return true;
        }
        if (climb.isInner && input.y > 0)
        {
            category = MovementMode.Climb;
            return true;
        }
        if (ladder.isInner && Mathf.Abs(input.y) > 0.1f)
        {
            category = MovementMode.Ladder;
            return true;
        }

        category = MovementMode.Jump;
        return false;
    }

    public override void Start()
    {
        //float jump = input.y >= 0f && inputJump ? jumpSpeed : 0f;
        //body.velocity = new Vector2(input.x * moveSpeed, jump);
    }

    public override void Update()
    {
        Vector2 velocity = body.velocity;
        velocity.x = input.x * moveSpeed;


        if (inputJump && isGrounded.Equals(false))
        {
            velocity.y = 0f;
            animator.SetTrigger("DoubleJump");
            velocity.y = jumpSpeed;
        }
        else if(Input.GetButtonUp("Jump") && velocity.y > 0f)
        {
            velocity.y *= 0.5f;
        }


        if (!inputJump && velocity.y < 0f)
        {
            velocity.y += Physics2D.gravity.y * 1.5f * Time.deltaTime;   
        }


        body.velocity = !body.isKinematic ? velocity : Vector2.zero;


        if (Mathf.Abs(input.x) > 0)
        {
            renderer.flipX = input.x < 0;
        }
    }
}
