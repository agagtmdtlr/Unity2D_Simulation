using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundMovement : PlayerControlState
{
    [SerializeField] Detection climb;
    [SerializeField] Detection ladder;

    [Header("Ground Info")]
    [SerializeField] float moveSpeed = 7.5f;
    [SerializeField] float jumpSpeed = 10f;

    public override Mode GetMode() { return Mode.Groud; }

    public override void Awake()
    {
        base.Awake();
    }

    public override void Exit()
    {
        //context.isGrounded = false;
    }

    public override void NeedChagne()
    {
        if(isGrounded == false)
        {
            context.ChangeState(Mode.Jump);
        }
        else if(Mathf.Abs( velocity.y) > 0)
        {
            context.ChangeState(Mode.Jump);
        }
        else if (climb.isInner && input.y > 0)
        {
            context.ChangeState(Mode.Climb);
        }
        else if (ladder.isInner && Mathf.Abs(input.y) > 0.1f)
        {
            float distance;
            bool isCorretToBegin = true;
            if (input.y < 0f)
            {
                float origin = collider2d.bounds.min.y;
                float to = ladder.bound.min.y;

                distance = Mathf.Abs(origin - to);
                if (distance < 0.2f)
                {
                    isCorretToBegin = false;
                }

            }
            else if (input.y > 0f)
            {
                float origin = collider2d.bounds.min.y;
                float to = ladder.bound.max.y + 0.1f;

                distance = Mathf.Abs(origin - to);
                if (distance < 0.2f)
                {
                    isCorretToBegin = false;
                }
            }

            if(isCorretToBegin)
                context.ChangeState(Mode.Ladder);
        }
        else if (inputR)
        {
            context.ChangeState(Mode.Inventory);
        }
        else if (inputInteract)
        {
            var sensor = context.interactor.DoInteract();
            if(sensor != null)
            {
                if(sensor.TryGetComponent(out MineCollectable mine))
                {
                    context.ChangeState(Mode.Mining);
                }
                else if(sensor.TryGetComponent(out WoodCollectable wood))
                {
                    context.ChangeState(Mode.WoodCutting);
                }
                else if( BuildingSystem.Instance.CurrentState.Equals(BuildState.Mode.SideMenu))
                {
                    context.ChangeState(Mode.Build);
                }
            }
        }
    }

    public override void Enter()
    {
    }

    public override void UpdateState()
    {
        Vector2 velocity = body.velocity;
        velocity.x = input.x * moveSpeed;
        velocity.y = inputJump && input.y >= 0 ? jumpSpeed : 0f;

        body.velocity = !body.isKinematic ? velocity : Vector2.zero;

        if (Mathf.Abs(input.x) > 0)
        {
            renderer2d.flipX = input.x < 0;
        }

    }
}
