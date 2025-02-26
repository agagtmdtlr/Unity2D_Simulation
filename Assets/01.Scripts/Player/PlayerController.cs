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
    [SerializeField] private bool climbingPlatform;
    [SerializeField] private ClimbDetection climbDetection;

    [Header("Ladder Info")]
    private bool climbingLadder;
    [SerializeField] LadderDetection ladderDetection;
    [SerializeField] LayerMask whatIsLadder;

    [SerializeField] private Vector2 velocity;
    [SerializeField] protected Vector2 groundNormal;
    float minDistanceToLadder = 0.5f;

    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;
    protected const float minGroundNormalY = 0.65f;

    float inputX;
    float inputY;
    // Start is called before the first frame update
    void Start()
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

    

    private void FixedUpdate()
    {
        

        
        float axisY_abs = Mathf.Abs(inputY);
        float axisX_abs = Mathf.Abs(inputX);

        if (climbingPlatform || climbingLadder)
        {
            return;
        }

        velocity.x = inputX * moveSpeed;


        if (!isGrounded)
        {
            velocity += Physics2D.gravity * Time.deltaTime;
        }

        //body.position = body.position + velocity * Time.deltaTime;
        var deltaPosition = velocity * Time.deltaTime;
        var moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);
        var move = moveAlongGround * deltaPosition.x;

        body.position = deltaPosition + body.position;
    }

    
    void Update()
    {
        inputY = Input.GetAxisRaw("Vertical");
        inputX = Input.GetAxisRaw("Horizontal");
        

        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("JumpSpeed", velocity.y);
        animator.SetFloat("GroundSpeed", Mathf.Abs(inputX));
        animator.SetFloat("dir_y", inputY);

        

        if (climbingPlatform || climbingLadder)
        {
            return;
        }

        bool inputJump = Input.GetKeyDown(KeyCode.Space);
        if (inputJump)
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
        }

        if(AvaibaleLadder())
        {
            StartLadder();
        }
    }

    bool AvaiableClimb()
    {
        

        if (climbDetection.throungBound && !climbingPlatform)
        {
            return true;
        }
        return false;
    }

    bool AvaibaleLadder()
    {
        // 사다리 타는 중이 아니고
        // 사다리가 가까이 있고
        // 수직 방향 입력이 발생했을때만
        if (ladderDetection.throughBound && !climbingLadder && Mathf.Abs(inputY) > 0.1f)
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
        if(collision.collider.CompareTag("Platform") && collision.contacts[0].normal.y > 0.7f)
        {
            isGrounded = true;
            velocity.y = 0;
            animator.SetBool("isGrounded", isGrounded);
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            isGrounded = false;
            animator.SetBool("isGrounded", isGrounded);
        }
    }


    bool ContinueLadder()
    {
        float yabs = Mathf.Abs(inputY);
        if(yabs < 0.1f)
        {
            return true;
        }

        RaycastHit2D hit2d = Physics2D.Raycast(collider2d.bounds.center, Vector2.up * Mathf.Sign(inputY),100f , whatIsLadder);
        return hit2d.distance > minDistanceToLadder;
    }

    public IEnumerator Ladder_co()
    {
        float start_x = ladderDetection.bound.center.x;
        var startPos = body.position;
        startPos.x = ladderDetection.bound.center.x;
        //startPos.y = Mathf.Clamp(startPos.y, ladderDetection.bound.min.y + 0.1f, ladderDetection.bound.max.y - 0.1f);
        body.position = startPos;
        animator.SetBool("ClimbLadder", climbingLadder);
        velocity = Vector2.zero;

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

        var endpos = body.position;
        //endpos.y += 0.1f;
        body.position = endpos;

        climbingLadder = false;
        animator.SetBool("ClimbLadder", climbingLadder);

        renderer2d.color = Color.white;

        yield break;
    }

    float Half(float y)
    {
        return y;
    }

    public void StartLadder()
    {
        Debug.Log("Start Ladder");
        climbingLadder = true;
        ladderDetection.throughBound = false;
        StopCoroutine("Ladder_co");
        StartCoroutine("Ladder_co");
    }

    public IEnumerator Climb_co()
    {
        renderer2d.color = Color.red;


        float platformY = climbDetection.bound.max.y + 0.1f;
        velocity = Vector2.zero;
        // start position
        var startPosition = body.position;
        float elapsedClimb = 0;

        Vector3 destination = new Vector3(startPosition.x, platformY - collider2d.size.y * 0.5f, 0);
        Vector3 normalizedPos = new Vector3(startPosition.x, platformY, 0f);

        animator.SetBool("ClimbPlatform", true);
        float climbMaxTime = 0.5f;

        while (elapsedClimb < climbMaxTime)
        {
            elapsedClimb += Time.deltaTime;
            float range = elapsedClimb / climbMaxTime;
            body.position = Vector3.Lerp(startPosition, destination, range);
            yield return null;
        }

        body.position = normalizedPos;
        climbingPlatform = false;
        animator.SetBool("ClimbPlatform", false);

        renderer2d.color = Color.white;

        yield break;
    }

    public void OnClimbFinished()
    {
        isGrounded = true;
        climbingPlatform = false;
        float platformY = climbDetection.bound.max.y;
        var startPosition = body.position;
        Vector3 normalizedPos = new Vector3(startPosition.x, platformY, 0f);
        body.position = normalizedPos;
        renderer2d.color = Color.white;
        animator.SetBool("ClimbPlatform", false);


    }

    public void StartClimb()
    {
        velocity = Vector2.zero;
        renderer2d.color = Color.red;
        var startPosition = body.position;
        float platformY = climbDetection.bound.max.y;
        platformY -= 1.323772f; // 하 이거 하드코딩해야되나 ㅠㅠ
        startPosition.y = platformY;
        body.position = startPosition;

        climbingPlatform = true;
        climbDetection.throungBound = false;
        animator.SetBool("ClimbPlatform", true);
        //StopCoroutine("Climb_co");
        //StartCoroutine("Climb_co");

    }


    private void OnGUI()
    {
        DebugTextManager.Write($"Velocity {velocity}");
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
