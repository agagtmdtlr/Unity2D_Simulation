using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline;
using UnityEngine;



public abstract class PlayerControlState : MonoBehaviour
{
    public enum Mode
    {
        Groud,
        Jump,
        Climb,
        Ladder,
        Inventory,
        Mining,       
        WoodCutting,
        Build,
        Cooking
    }

    public PlayerControlContext context;
    protected Animator animator;
    protected Rigidbody2D body;
    protected Collider2D collider2d;
    protected SpriteRenderer renderer2d;
    protected Controllable control;

    protected bool isGrounded { get { return context.isGrounded; } }
    protected bool inputR { get { return control.InputR; } }
    protected bool inputInteract { get { return control.InputInteract; } }
    protected Vector2 input { get { return control.Axis; } }
    protected Vector2 input_Abs { get { return control.Axis_Abs; } }
    protected Vector2 velocity { get { return body.velocity; } set { body.velocity = value; } }
    protected bool inputJump { get { return control.InputJump; } }

    public virtual void Awake()  
    {  
        context.TryGetComponent(out animator);
        context.TryGetComponent(out body);
        context.TryGetComponent(out collider2d);
        context.TryGetComponent(out renderer2d);
        context.TryGetComponent(out control);
    }

    public abstract Mode GetMode();
    public abstract void UpdateState();
    public abstract void Enter();
    public abstract void Exit();
    public abstract void NeedChagne();
}
