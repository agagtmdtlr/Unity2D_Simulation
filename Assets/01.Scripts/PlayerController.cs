using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpSpeed = 700f;
    [SerializeField] private Rigidbody2D rigid2d;
    [SerializeField] private BoxCollider2D collider;
    [SerializeField] private SpriteRenderer renderer;

    [SerializeField] private GameObject animObj;

    [SerializeField] private Text debugtxt;

    [SerializeField] private bool isGrounded;

    [SerializeField] private bool climbPlatform;
    [SerializeField] private bool throughClimbVolume;
    [SerializeField] Bounds platformBound;

    private bool throughLadder;
    private bool climbLadder;
    [SerializeField] Bounds ladderBound;

    // Start is called before the first frame update
    void Start()
    {
        animObj.TryGetComponent(out animator);
        animObj.TryGetComponent(out renderer);

        TryGetComponent(out collider);
        TryGetComponent(out rigid2d);

        isGrounded = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(climbPlatform || climbLadder)
        {
            return;
        }

        float axisY = Input.GetAxisRaw("Vertical");
        float axisY_abs = Mathf.Abs(axisY);

        float axisX = Input.GetAxisRaw("Horizontal");
        float axisX_abs = Mathf.Abs(axisX);

        transform.Translate(Vector2.right * axisX * moveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("start jump");

            if (isGrounded.Equals(false))
            {
                rigid2d.velocity = Vector2.zero;
                animator.SetTrigger("DoubleJump");
            }

            rigid2d.AddForce(Vector2.up * jumpSpeed);
            //Debug.Log( $"Jump {rigid2d.velocity}");
        }

        if( Input.GetKeyUp(KeyCode.Space) && rigid2d.velocity.y > 0)
        {
            var v = rigid2d.velocity;
            v.y = v.y * 0.5f;
            rigid2d.velocity = v;
        }


        animator.SetFloat("JumpSpeed", rigid2d.velocity.y);
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
        debugtxt.text = $"Velocity {rigid2d.velocity}";
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
            Debug.Log("collision enter");
            animator.SetBool("isGrounded", isGrounded);
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            Debug.Log("collision exit");
            isGrounded = false;
            animator.SetBool("isGrounded", isGrounded);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Climb"))
        {
            Debug.Log("trigger enter");
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

    }

    bool ContinueLadder()
    {
        float offY = Mathf.Abs(transform.position.y - ladderBound.center.y);
        float diffY = (ladderBound.size.y) - offY;
        return diffY > 0.01f;
    }
    public IEnumerator Ladder_co()
    {
        float start_x = ladderBound.center.x;
        var startPos = transform.position;
        startPos.x = ladderBound.center.x;
        animator.SetBool("ClimbLadder", climbLadder);

        while (ContinueLadder())
        {
            float axisY = Input.GetAxisRaw("Vertical");
            transform.Translate(Vector2.up * axisY * moveSpeed * Time.deltaTime);
            animator.SetFloat("dir_y", axisY);

            yield return null;
        }

        climbLadder = false;
        animator.SetBool("ClimbLadder", climbLadder);
        yield break;
    }

    public void StartLadder()
    {
        climbLadder = true;
        throughLadder = false;
        StopCoroutine("Ladder_co");
        StartCoroutine("Ladder_co");
    }

    public IEnumerator Climb_co()
    {
        float platformY = platformBound.max.y + 0.1f;
        rigid2d.velocity = Vector2.zero;
        // start position
        var startPosition = transform.position;
        float elapsedClimb = 0;

        Vector3 destination = new Vector3(startPosition.x, platformY - collider.size.y * 0.5f, startPosition.z);
        Vector3 normalizedPos = new Vector3(startPosition.x, platformY, startPosition.z);

        animator.SetBool("ClimbPlatform", true);
        float climbMaxTime = 0.5f;

        while (elapsedClimb < climbMaxTime)
        {
            elapsedClimb += Time.deltaTime;
            float range = elapsedClimb / climbMaxTime;
            transform.position = Vector3.Lerp(startPosition, destination, range);
            yield return null;
        }

        transform.position = normalizedPos;
        climbPlatform = false;
        animator.SetBool("ClimbPlatform", false);


        yield break;
    }

    public void StartClimb()
    {
        climbPlatform = true;
        throughClimbVolume = false;
        StopCoroutine("Climb_co");
        StartCoroutine("Climb_co");

    }

    public void OnClimbComplete()
    {
        climbPlatform = false;
    }

    private void OnDrawGizmos()
    {
        
    }
}
