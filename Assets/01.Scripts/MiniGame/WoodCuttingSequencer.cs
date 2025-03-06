using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WoodCuttingSequencer : MonoBehaviour
{
    GameObject tool;
    GameObject player;
    Harvestable target;
    Rigidbody2D rb;
    Collider2D col;
    Controllable control;

    [SerializeField] float startPosXOffset;

    Animator animator;

    float inputX;
    float progress = 0f;
    int progressDirection;
    [SerializeField] float progressMax = 10f;
    [SerializeField] float fillSpeed = 5f;
    bool isPlaying = false;



    private void Awake()
    {
    }

    public void OnStopSequencer()
    {
        isPlaying = false;

        rb.isKinematic = false;
        control.UnLock(this);
        animator.SetBool("Wood", false);

        gameObject.SetActive(false);

        tool.SetActive(false);

    }

    public void OnPlaySequencer(GameObject _player , Harvestable _target)
    {
        isPlaying = true;


        this.player = _player;
        this.target = _target;

        gameObject.SetActive(true);
        gameObject.transform.position = player.transform.position;


        if ( player.TryGetComponent(out rb) )
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
        }

        if( player.TryGetComponent(out control))
        {
            control.Lock(this);
        }

        if (player.TryGetComponent(out animator))
        {
            animator.SetBool("Wood", true);
        }

        if (player.TryGetComponent(out col))
        {

        }

        tool = player.transform.Find("tool_saw").gameObject;
        tool.SetActive(true);

        // init player position
        Vector3 startPos = col.bounds.center;
        startPos.x = target.transform.position.x + startPosXOffset;
        startPos.y = rb.position.y;
        rb.position = startPos;

        progressDirection = 1;
    }

    void Update()
    {
        if (!isPlaying)
            return;


        inputX = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("dir_x", inputX);


        if ((int)inputX == progressDirection)
        {
            progress += fillSpeed * Time.deltaTime;
        }

        rb.position += Vector2.right * inputX * fillSpeed * Time.deltaTime;

        // 끝지점에 도달했다면 방향을 바꿔준다.
        if(progress >= progressMax)
        {
            target.IncreamentStage();

            if(target.isEndState())
            {
                OnStopSequencer();
            }

            progressDirection = -progressDirection;
            progress = 0;
        }
    }
}
