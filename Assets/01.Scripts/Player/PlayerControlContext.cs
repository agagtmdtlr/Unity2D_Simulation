using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityEngine.UI;

public class PlayerControlContext : MonoBehaviour
{
    public Color defaultColor = Color.green;

    [Header("Camera")]
    public CinemachineVirtualCamera virtualCamera;
    public float lensSize = 7f;

    [Header("Ground Info")]
    public bool isGrounded = false;

    public Vector2 boxSize;
    public float castDistance;
    public LayerMask whatisGround;

    [Header("Interaction")]
    public Interactor interactor;

    [Header("Control State")]
    [SerializeField] GameObject stateObj;

    [Header("Inventory")]
    public ItemStorage itemStorage;


    Controllable control;
    Animator animator;
    Rigidbody2D body;
    BoxCollider2D collider2d;

    PlayerControlState.Mode currentMode;
    Dictionary<PlayerControlState.Mode, PlayerControlState> stateContainer;


    public void ChangeState(PlayerControlState.Mode mode)
    {
        if(currentMode != mode)
        {
            stateContainer[currentMode].Exit();
            currentMode = mode;
            stateContainer[currentMode].Enter();
        }
    }

    private void Awake()
    {
        stateContainer = new Dictionary<PlayerControlState.Mode, PlayerControlState>();
        currentMode = PlayerControlState.Mode.Groud;
        var states = stateObj.GetComponents<PlayerControlState>();
        foreach (var state in states)
        {
            stateContainer[state.GetMode()] = state;
        }
    }

    private void Start()
    {
        TryGetComponent(out animator);
        TryGetComponent(out collider2d);
        TryGetComponent(out body);
        TryGetComponent(out control);

        isGrounded = false;

    }

    private void Update()
    {
        if(!control.InputLocked )
        {
            isGrounded = IsGrounded();
            animator.SetBool("isGrounded", isGrounded);
            animator.SetFloat("GroundSpeed", body.velocity.x);
            animator.SetFloat("JumpSpeed", body.velocity.y);
            animator.SetFloat("dir_x", control.Axis.x); 
            animator.SetFloat("dir_y", control.Axis.y);
        }

        stateContainer[currentMode].UpdateState();
        stateContainer[currentMode].NeedChagne();
    }


    

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
