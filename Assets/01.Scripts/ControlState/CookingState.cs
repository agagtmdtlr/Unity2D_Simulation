using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CookingState : PlayerControlState
{
    CookingHandler cook;

    public override void Enter()
    {
        cook = context.interactor.Interacted.GetComponent<CookingHandler>();
    }

    public override void Exit()
    {
    }

    public override Mode GetMode()
    {
        return Mode.Cooking;
    }

    public override void NeedChagne()
    {
        if(!cook.Interacting)
        {
            context.ChangeState(Mode.Groud);
        }
    }

    public override void UpdateState()
    {
    }
}
