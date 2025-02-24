using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpSpeed = 700f;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private BoxCollider2D collider;
    [SerializeField] private SpriteRenderer renderer;

    [SerializeField] private Text debugtxt;

    [SerializeField] private bool isGrounded;

    [SerializeField] private bool climbPlatform;
    [SerializeField] private bool throughClimbVolume;
    [SerializeField] Bounds platformBound;

    private bool throughLadder;
    private bool climbLadder;
    [SerializeField] Bounds ladderBound;

    [SerializeField] private Vector2 velocity;
    [SerializeField] protected Vector2 groundNormal;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;
    protected const float minGroundNormalY = 0.65f;
    // Start is called before the first frame update
    void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;

        TryGetComponent(out animator);
        TryGetComponent(out renderer);

        TryGetComponent(out collider);
        TryGetComponent(out body);

        isGrounded = false;
    }

    

    private void FixedUpdate()
    {
        if (climbPlatform || climbLadder)
        {
            return;
        }

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
        if(climbPlatform || climbLadder)
        {
            return;
        }
        animator.SetBool("isGrounded", isGrounded);

        float axisY = Input.GetAxisRaw("Vertical");
        float axisY_abs = Mathf.Abs(axisY);

        float axisX = Input.GetAxisRaw("Horizontal");
        float axisX_abs = Mathf.Abs(axisX);

        velocity.x = axisX * moveSpeed;        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded.Equals(false))
            {
                velocity.y = 0f;
                animator.SetTrigger("DoubleJump");
            }

            velocity.y = jumpSpeed;
        }

        if( Input.GetKeyUp(KeyCode.Space) && velocity.y > 0)
        {
            velocity.y = velocity.y * 0.5f;
        }

        


        
        animator.SetFloat("JumpSpeed", velocity.y);
        animator.SetFloat("GroundSpeed", Mathf.Abs(axisX_abs));
        animator.SetFloat("dir_y", axisY);

        Vector3 default_scale = Vector3.one;
        if(axisX_abs > 0)
        {
            renderer.flipX = axisX < 0;
        }

        if(axisY > 0 && AvaiableClimb())
        {
            StartClimb();
        }

        if(axisY > 0 && AvaibaleLadder())
        {
            StartLadder();
        }
        debugtxt.text = $"Velocity {velocity}";
    }

    bool AvaiableClimb()
    {
        if( throughClimbVolume )
        {
            return true;
        }
        return false;
    }

    bool AvaibaleLadder()
    {
        if(throughLadder)
        {
            return true;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Climb"))
        {
            throughClimbVolume = true;

            if( collision.transform.parent.TryGetComponent(out Collider2D platformColl ))
            {
                platformBound = platformColl.bounds;
            }
            else
            {
                Debug.LogAssertion("failed get parent collider");
            }
        }
        
        if(collision.CompareTag("Ladder"))
        {
            throughLadder = true;
            ladderBound = collision.bounds;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.CompareTag("Climb"))
        {
            throughClimbVolume = false;
        }

        if (collision.CompareTag("Ladder"))
        {
            throughLadder = false;
        }

    }

    bool ContinueLadder()
    {
        float offY = Mathf.Abs(body.position.y - ladderBound.center.y);
        float diffY = (ladderBound.size.y * 0.5f) - offY;
        return diffY > 0.01f;
    }
    public IEnumerator Ladder_co()
    {
        float start_x = ladderBound.center.x;
        var startPos = body.position;
        startPos.x = ladderBound.center.x;
        body.position = startPos;
        animator.SetBool("ClimbLadder", climbLadder);
        velocity = Vector2.zero;

        renderer.color = Color.yellow;

        while (ContinueLadder())
        {
            float axisY = Input.GetAxisRaw("Vertical");
            body.position = body.position + Vector2.up * axisY * moveSpeed * Time.deltaTime;
            animator.SetFloat("dir_y", axisY);

            yield return null;
        }

        var endpos = body.position;
        endpos.y += 0.1f;
        body.position = endpos;

        climbLadder = false;
        animator.SetBool("ClimbLadder", climbLadder);

        renderer.color = Color.white;


        yield break;
    }

    public void StartLadder()
    {
        Debug.Log("Start Ladder");
        climbLadder = true;
        throughLadder = false;
        StopCoroutine("Ladder_co");
        StartCoroutine("Ladder_co");
    }

    public IEnumerator Climb_co()
    {
        renderer.color = Color.red;


        float platformY = platformBound.max.y + 0.1f;
        velocity = Vector2.zero;
        // start position
        var startPosition = body.position;
        float elapsedClimb = 0;

        Vector3 destination = new Vector3(startPosition.x, platformY - collider.size.y * 0.5f, 0);
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
        climbPlatform = false;
        animator.SetBool("ClimbPlatform", false);

        renderer.color = Color.white;

        yield break;
    }

    public void OnClimbFinished()
    {
        isGrounded = true;
        climbPlatform = false;
        float platformY = platformBound.max.y;
        var startPosition = body.position;
        Vector3 normalizedPos = new Vector3(startPosition.x, platformY, 0f);
        body.position = normalizedPos;
        renderer.color = Color.white;
        animator.SetBool("ClimbPlatform", false);


    }

    public void StartClimb()
    {
        velocity = Vector2.zero;
        renderer.color = Color.red;
        var startPosition = body.position;
        float platformY = platformBound.max.y;
        platformY -= 1.323772f; // 하 이거 하드코딩해야되나 ㅠㅠ
        startPosition.y = platformY;
        body.position = startPosition;

        climbPlatform = true;
        throughClimbVolume = false;
        animator.SetBool("ClimbPlatform", true);
        //StopCoroutine("Climb_co");
        //StartCoroutine("Climb_co");

    }

    private void OnDrawGizmos()
    {
    }
}
