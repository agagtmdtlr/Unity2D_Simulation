using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionControl : PlayerControlState
{
    
    public override void Enter()
    {
        body.isKinematic = true;
        body.velocity = Vector3.zero;
    }

    public override void Exit()
    {
        body.isKinematic = false;
    }

    public override Mode GetMode()
    {
        return Mode.Build;
    }

    public override void NeedChagne()
    {
        if(BuildingSystem.Instance.CurrentState.Equals(BuildState.Mode.None))
        {
            context.ChangeState(Mode.Groud);
        }
    }

    public override void UpdateState()
    {
    }
}
