using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum InteractConsumeTime
{
    Immediate,
    Manual
}
public enum InteractConsumeLife
{
    Once,
    ReUsable
}

public class InteractEvent
{
    public delegate void InteractHandler();
    public event InteractHandler HasInteracted; // 
    public void CallInteractEvent() => HasInteracted?.Invoke();
}

public class Interactable : MonoBehaviour
{
    [HideInInspector]
    public bool consumed { get; private set; } = false;
    [SerializeField] private InteractConsumeLife consumeType;
    [SerializeField] private InteractConsumeTime consumedTime;
    public InteractConsumeTime ConsumeTime { get; private set; }
    [SerializeField] private IToggle toggle = new IToggleDefault();
    [SerializeField] private LayerMask[] whatIsInteractor;

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

        if(consumeType == InteractConsumeLife.Once) 
            consumed = true;

        this._interactor = interactor;
        interaction.CallInteractEvent();
    }


    public void FocusIn()
    {
        toggle.FocusIn();
    }

    public void FocusOut()
    {
        toggle.FocusOut();
    }

    protected virtual void Awake()
    {
        LayerMask finalMask = 0;

        foreach(var layer in whatIsInteractor)
        {
            finalMask |= layer;
        }

        TryGetComponent(out collider2d);
        collider2d.callbackLayers = finalMask;
    }

    private void OnEnable()
    {
        consumed = false;        
    }
}
