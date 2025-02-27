using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityEngine.UI;

public enum MovementState
{
    Ground,
    Jump,
    Climb,
    Ladder
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Text debugtxt;
    [SerializeField] private Animator animator;

    [Header("Ground Info")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpSpeed = 700f;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private BoxCollider2D collider2d;
    [SerializeField] private SpriteRenderer renderer2d;
    [SerializeField] private bool isGrounded;

    [SerializeField] bool inputJump;

    [Header("Climb Info")]
    [SerializeField] private ClimbDetection climbDetection;
    [SerializeField] private float climbBeginPosYOffset = 0.0f;
    [SerializeField] private float climbEndPosYOffset = 0.0f;

    [Header("Ladder Info")]
    [SerializeField] LadderDetection ladderDetection;
    [SerializeField] LayerMask whatIsLadder;
    float minDistanceToLadder = 0.5f;

    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;
    protected const float minGroundNormalY = 0.65f;

    float inputX;
    float inputY;
    float inputX_Abs;
    float inputY_Abs;

    public bool inputLocked = false;

    private void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;

        TryGetComponent(out animator);
        TryGetComponent(out renderer2d);

        TryGetComponent(out collider2d);
        TryGetComponent(out body);

        ladderDetection = GetComponentInChildren<LadderDetection>();
        climbDetection = GetComponentInChildren<ClimbDetection>();

        isGrounded = false;
    }

    private void Update()
    {
        if(!inputLocked)
        {
            inputY = Input.GetAxisRaw("Vertical");
            inputX = Input.GetAxisRaw("Horizontal");
            inputY_Abs = Mathf.Abs(inputY);
            inputX_Abs = Mathf.Abs(inputX);

            animator.SetBool("isGrounded", isGrounded);
            animator.SetFloat("JumpSpeed", inputY);
            animator.SetFloat("GroundSpeed", Mathf.Abs(inputX));
            animator.SetFloat("dir_y", inputY); 
        }

        if (body.isKinematic)
        {
        }
        else
        {
            Vector2 velocity = body.velocity;
            velocity.x = inputX * moveSpeed;

            bool inputJump = Input.GetKeyDown(KeyCode.Space);
            if (inputJump && inputY >= 0f)
            {
                if (isGrounded.Equals(false))
                {
                    velocity.y = 0f;
                    animator.SetTrigger("DoubleJump");
                }

                velocity.y = jumpSpeed;
            }

            if (Input.GetKeyUp(KeyCode.Space) && velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }

            if (Mathf.Abs(inputX) > 0)
            {
                renderer2d.flipX = inputX < 0;
            }

            if (inputY > 0 && AvaiableClimb())
            {
                StartClimb();
                return;
            }

            if (AvaibaleLadder())
            {
                StartLadder();
                return;
            }

            body.velocity = velocity;
        }
    }

    private bool AvaiableClimb()
    {
        if (climbDetection.throungBound)
        {
            return true;
        }
        return false;
    }

    private bool AvaibaleLadder()
    {
        // 사다리 타는 중이 아니고
        // 사다리가 가까이 있고
        // 수직 방향 입력이 발생했을때만
        //if (ladderDetection.throughBound && !climbingLadder && Mathf.Abs(inputY) > 0.1f)
        if (ladderDetection.throughBound && Mathf.Abs(inputY) > 0.1f)
        {
            RaycastHit2D hit2d = Physics2D.Raycast(collider2d.bounds.center, Vector2.up * Mathf.Sign(inputY), 100f, whatIsLadder);
            float minDistanceToLadder = 0.5f; // 너무 가까우면 무시한다.
            if (hit2d.collider != null && hit2d.distance > minDistanceToLadder)
            {
                return true;
            }
        }

        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.7f)
            isGrounded = true;

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    private bool ContinueLadder()
    {
        float yabs = Mathf.Abs(inputY);
        if(yabs < 0.1f)
        {
            return true;
        }

        RaycastHit2D hit2d = Physics2D.Raycast(collider2d.bounds.center, Vector2.up * Mathf.Sign(inputY),100f , whatIsLadder);
        return hit2d.distance > minDistanceToLadder;
    }

    private IEnumerator ClimbingLadder_co()
    {
        float start_x = ladderDetection.bound.center.x;
        var startPos = body.position;
        startPos.x = ladderDetection.bound.center.x;
        body.position = startPos;
        animator.SetBool("ClimbLadder", true);

        float height = collider2d.bounds.size.y;
        var toppos = ladderDetection.bound.max.y + height;
        var bottompos = ladderDetection.bound.min.y + height;


        renderer2d.color = Color.yellow;

        while (ContinueLadder())
        {
            body.position = body.position + Vector2.up * inputY * moveSpeed * Time.deltaTime;
            animator.SetFloat("dir_y", inputY);

            yield return null;
        }
        EndLadder();
        yield break;
    }

    private void EndLadder()
    {
        var endpos = body.position;
        body.position = endpos;
        body.isKinematic = false;


        animator.SetBool("ClimbLadder", false);

        renderer2d.color = Color.white;
    }

    private void StartLadder()
    {
        body.velocity = Vector2.zero;
        body.isKinematic = true;
        Debug.Log("Start Ladder");
        //climbingLadder = true;
        ladderDetection.throughBound = false;
        StopCoroutine("ClimbingLadder_co");
        StartCoroutine("ClimbingLadder_co");
    }

    private void StartClimb()
    {
        body.velocity = Vector2.zero;
        body.isKinematic = true;

        // set climb start position with offset Y
        var startPosition = body.position;
        float platformY = climbDetection.bound.max.y;
        platformY += climbBeginPosYOffset;
        startPosition.y = platformY;
        body.position = startPosition;

        climbDetection.throungBound = false;
        animator.SetBool("ClimbPlatform", true);

        // set debug anim color
        renderer2d.color = Color.red;

    }


    private void EndClimb()
    {
        body.isKinematic = false;

        // set climb end position with offset Y
        float platformY = climbDetection.bound.max.y;
        Vector3 endPosition= new Vector3(body.position.x, platformY + climbEndPosYOffset, 0f);
        
        body.position = endPosition;

        animator.SetBool("ClimbPlatform", false);

        renderer2d.color = Color.white;

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


    }
}
