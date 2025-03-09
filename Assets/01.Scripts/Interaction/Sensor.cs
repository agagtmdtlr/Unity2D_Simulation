using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class FocusAction : MonoBehaviour
{
    public abstract void FocusIn(Transform toFocus);
    public abstract void FocusOut(Transform toFocus);
}
public interface IFocusAction
{
    public void FocusIn();
    public void FocusOut();
}
public enum InteractConsumeLife
{
    Once,
    ReUsable
}

[System.Flags]
public enum InteractTriggerWay
{
    EnterTrigger = 1 << 0,
    StayTrigger = 1 << 1,
    OutTrigger = 1 << 2,
    Manual = 1 << 3
}

[System.Serializable]
public class SensorTriggerEvent : UnityEvent<SensorTriggeredEventArgs> { }

public class SensorTriggeredEventArgs
{
    public Sensor sender;

    public SensorTriggeredEventArgs(Sensor sender)
    {
        this.sender = sender;
    }
}

public class Sensor : MonoBehaviour
{
    public bool consumed { get; private set; } = false;
    [Tooltip("상호작용 수명( 1회용, 재사용 )")]
    [SerializeField] private InteractConsumeLife consumeLife;

    [SerializeField] private InteractTriggerWay triggerWay;
    public InteractTriggerWay TriggerWay { get { return triggerWay; } }

    [SerializeField] private IFocusAction focusAction = null;
    [SerializeField] private LayerMask whatIsInteractor;

    public UnityEvent<Sensor> interactEvent;

    Dictionary<InteractTriggerWay, UnityEvent<Sensor>> interactActions;


    Collider2D collider2d;

    private Interactor interactor;
    public Interactor Interactor
    {
        get { return interactor; }
    }

    public void CallInteract(Interactor interactor)
    {
        if(consumed) 
            return;

        if(consumeLife == InteractConsumeLife.Once) 
            consumed = true;

        this.interactor = interactor;
        interactEvent.Invoke(this);
    }

    public void FocusIn()
    {
        if(focusAction != null)
            focusAction.FocusIn();

    }

    public void FocusOut()
    {
        if (focusAction != null)
            focusAction.FocusOut();
    }


    protected virtual void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Interaction");

        TryGetComponent(out collider2d);
        TryGetComponent(out focusAction);


    }
    private void OnEnable()
    {
        consumed = false;

        if(collider2d != null)
            collider2d.callbackLayers |= whatIsInteractor;
    }

    void Consume()
    {
        consumed = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( triggerWay.HasFlag(InteractTriggerWay.EnterTrigger) && !consumed)
        {
             if (collision.TryGetComponent(out Interactor interactor))
            {
                CallInteract(interactor);
                Consume();
            }
        }
        else if( triggerWay.HasFlag(InteractTriggerWay.Manual) && !consumed)
        {
            if (collision.TryGetComponent(out Interactor interactor))
            {
                interactor.AddSenser(this);
                FocusIn();
            }
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(triggerWay.HasFlag(InteractTriggerWay.StayTrigger) && !consumed)
        {
            if(collision.TryGetComponent(out Interactor interactor))
            {
                CallInteract(interactor);
                Consume();
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (triggerWay.HasFlag(InteractTriggerWay.OutTrigger) && !consumed)
        {
            if (collision.TryGetComponent(out Interactor interactor))
            {
                CallInteract(interactor);
                Consume();
            }
        }
        else if (triggerWay.HasFlag(InteractTriggerWay.Manual))
        {
            if (collision.TryGetComponent(out Interactor interactor))
            {
                interactor.RemoveSensor(this);
                FocusOut();
            }
        }
    }


}
