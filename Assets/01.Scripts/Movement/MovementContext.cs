using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum MovementCategory
{
    Groud,
    Jump,
    Climb,
    Ladder
}
public abstract class MovementState
{
    public PlayerController context;
    protected Animator animator;
    protected Rigidbody2D body;
    protected Collider2D collider;
    protected SpriteRenderer renderer;
    protected Controllable control;

    protected bool isGrounded { get { return context.isGrounded; } }
    protected Vector2 input { get { return control.Axis; } }
    protected Vector2 input_Abs { get { return control.Axis_Abs; } }
    protected Vector2 velocity { get { return body.velocity; } set { body.velocity = value; } }
    protected bool inputJump { get { return control.InputJump; } }
    protected float moveSpeed { get { return context.moveSpeed; } }
    protected float jumpSpeed { get { return context.jumpSpeed; } }

    public MovementState(PlayerController context) 
    {  
        this.context = context;
        context.TryGetComponent(out animator);
        context.TryGetComponent(out body);
        context.TryGetComponent(out collider);
        context.TryGetComponent(out renderer);
        context.TryGetComponent(out control);
    }

    public abstract void Update();
    public abstract void Start();
    public abstract void End();

    public abstract bool NeedChagne(out MovementCategory category);
    public void Check()
    {
        if (NeedChagne(out MovementCategory category))
        {
            context.ChangeState(category);
        }
    }
}
