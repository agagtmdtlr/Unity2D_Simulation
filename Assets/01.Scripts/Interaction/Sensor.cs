using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IInteractAction
{
    public void CallInteract(Interactor interactor);
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

public enum InteractTriggerWay
{
    EnterTrigger,
    StayTrigger,
    OutTrigger,
    Manual
}

public class InteractEvent
{
    public delegate void InteractHandler();
    public event InteractHandler HasInteracted; // 
    public void CallInteractEvent() => HasInteracted?.Invoke();
}

public class Sensor : MonoBehaviour
{
    public bool consumed { get; private set; } = false;
    [Tooltip("상호작용 수명( 1회용, 재사용 )")]
    [SerializeField] private InteractConsumeLife consumeLife;

    [SerializeField] private InteractTriggerWay triggerWay;
    public InteractTriggerWay TriggerWay { get { return triggerWay; } }

    [SerializeField] private IFocusAction focusAction = null;
    [SerializeField] private LayerMask[] whatIsInteractor;

    public UnityEvent interactEvent;

    Collider2D collider2d;


    protected InteractEvent interaction = new InteractEvent();
    public InteractEvent Interact
    {
        get
        {
            if (interaction == null) interaction = new InteractEvent();
            return interaction;
        }
    }

    private Interactor _interactor;
    public Interactor interactor
    {
        get { return _interactor; }
    }


    public virtual void CallInteract(Interactor interactor)
    {
        if(consumed) 
            return;

        if(consumeLife == InteractConsumeLife.Once) 
            consumed = true;

        this._interactor = interactor;
        interactEvent.Invoke();
        //interaction.CallInteractEvent();
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
        LayerMask finalMask = 0;

        foreach (var layer in whatIsInteractor)
        {
            finalMask |= layer;
        }

        if(collider2d != null)
            collider2d.callbackLayers |= finalMask;
    }

    void Consume()
    {
        consumed = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggerWay.Equals(InteractTriggerWay.EnterTrigger) && !consumed)
        {
             if (collision.TryGetComponent(out Interactor interactor))
            {
                CallInteract(interactor);
                Consume();
            }

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(triggerWay.Equals(InteractTriggerWay.StayTrigger) && !consumed)
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
        if (triggerWay.Equals(InteractTriggerWay.OutTrigger) && !consumed)
        {
            if (collision.TryGetComponent(out Interactor interactor))
            {
                CallInteract(interactor);
                Consume();
            }

        }
    }


}
