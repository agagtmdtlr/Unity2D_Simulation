using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Ground Info")]
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float jumpSpeed = 700f;

    [SerializeField] public bool isGrounded = false;

    [Header("Jump Info")]
    public bool inputJump;

    [Header("Climb Info")]
    public Detection climbDetection;
    public float climbBeginPosYOffset = 0.0f;
    public float climbEndPosYOffset = 0.0f;

    [Header("Ladder Info")]
    public Detection ladderDetection;

    [Header("Input Command")]
    public Vector2 input;
    public Vector2 input_Abs;
    public bool inputLocked = false;


    Animator animator;
    Rigidbody2D body;
    BoxCollider2D collider2d;
    Interactor interactor;

    MovementCategory currentCategory;
    Dictionary<MovementCategory, MovementState> stateContainer;
    public void ChangeState(MovementCategory toCategory)
    {
        stateContainer[currentCategory].End();
        currentCategory = toCategory;
        stateContainer[currentCategory].Start();
    }

    private void Start()
    {
        TryGetComponent(out animator);
        TryGetComponent(out collider2d);
        TryGetComponent(out body);
        TryGetComponent(out interactor);

        isGrounded = false;

        if (ladderDetection == null || climbDetection == null)
        {
            Debug.LogAssertion("반드시 detection을 지정해주어야 합니다.");
        }

        stateContainer = new Dictionary<MovementCategory, MovementState>();
        currentCategory = MovementCategory.Groud;

        stateContainer[MovementCategory.Groud] = 
            new GroundMovement(this, climbDetection,ladderDetection);
        stateContainer[MovementCategory.Jump] = 
            new JumpMovement(this, climbDetection,ladderDetection);
        stateContainer[MovementCategory.Climb] = 
            new ClimbMovement(this, climbDetection, climbBeginPosYOffset, climbEndPosYOffset);
        stateContainer[MovementCategory.Ladder] = 
            new LadderMovement(this, ladderDetection);
    }

    private void Update()
    {
        if(!inputLocked )
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            input_Abs.x = Mathf.Abs(input.x);
            input_Abs.y = Mathf.Abs(input.y);

            inputJump = Input.GetButtonDown("Jump");

            isGrounded = IsGrounded();
            animator.SetBool("isGrounded", isGrounded);
            animator.SetFloat("GroundSpeed", body.velocity.x);
            animator.SetFloat("JumpSpeed", body.velocity.y);
            animator.SetFloat("dir_x", input.x); 
            animator.SetFloat("dir_y", input.y);
        }

        stateContainer[currentCategory].Update();
        stateContainer[currentCategory].Check();
    }


    public Vector2 boxSize;
    public float castDistance;
    public LayerMask whatisGround;



    bool IsGrounded()
    {
        var hit = Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, whatisGround);
        if (hit) 
        {
            return hit.normal.y > 0.7f;
        }
        else
        {
            return false;
        }

    }

    
    private void OnGUI()
    {
        if(body)
            DebugTextManager.Write($"Velocity {body.velocity}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if(body)
            Gizmos.DrawWireSphere(body.position, 0.1f);
        Gizmos.color = Color.green;
        if(collider2d)
            Gizmos.DrawWireSphere(collider2d.bounds.center, 0.1f);

        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
    }
}
