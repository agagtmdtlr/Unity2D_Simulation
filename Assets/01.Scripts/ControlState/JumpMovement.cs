using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpMovement : PlayerControlState
{
    
    [Header("Ground Info")]
    public float moveSpeed = 7.5f;
    public float jumpSpeed = 10f;

    [Header("Platform Detection")]
    [SerializeField] Detection climb;
    [SerializeField] Detection ladder;

    public override void Awake()
    {
        base.Awake();
    }

    public override Mode GetMode() { return Mode.Jump; }

    public override void Exit()
    {
        //context.isGrounded = true;
    }

    public override void NeedChagne()
    {
        if(Mathf.Abs(velocity.y) < 0.01f)
        {
            context.ChangeState(Mode.Groud);
        }
        else if (climb.isInner && input.y > 0)
        {
            context.ChangeState(Mode.Climb);
        }
        else if (ladder.isInner && Mathf.Abs(input.y) > 0.1f)
        {
            context.ChangeState(Mode.Ladder);
        }
    }

    public override void Enter()
    {
        float jump = inputJump && input.y >= 0 ? jumpSpeed : 0f;
        body.velocity = new Vector2(input.x * moveSpeed, jump);
    }

    public override void UpdateState()
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
            renderer2d.flipX = input.x < 0;
        }
    }
}
