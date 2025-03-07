using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbMovement : MovementState
{
    Detection detection;
    float beginPosYOffset = 0.0f;
    float endPosYOffset = 0.0f;

    public ClimbMovement(PlayerController context,
        Detection detection,
        float beginPosYOffset,
        float endPosYOffset
        )
        : base(context)
    {
        this.detection = detection;
        this.beginPosYOffset = beginPosYOffset;
        this.endPosYOffset = endPosYOffset;
    }

    public override void End()
    {
        // set climb end position with offset Y
        float platformY = detection.bound.max.y;
        Vector3 endPosition = 
            new Vector3(body.position.x, platformY + endPosYOffset, 0f);
        body.position = endPosition;

        animator.SetBool("ClimbPlatform", false);

        renderer.color = Color.white;
        body.isKinematic = false;
    }

    public override bool NeedChagne(out MovementMode category)
    {
        category = MovementMode.Climb;
        if (animator.GetBool("ClimbPlatform") == false)
        {
            category = MovementMode.Groud;
            return true;
        }

        return false;
    }

    public override void Start()
    {
        body.isKinematic = true;
        velocity = Vector2.zero;

        // set climb start position with offset Y
        var startPosition = body.position;
        float platformY = detection.bound.max.y;
        platformY += beginPosYOffset;
        startPosition.y = platformY;
        body.position = startPosition;

        animator.SetBool("ClimbPlatform", true);

        // set debug anim color
        renderer.color = Color.red;
    }

    public override void Update()
    {
        

    }

}
