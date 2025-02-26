using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class InteractEvent
{
    public delegate void InteractHandler();
    public event InteractHandler HasInteracted; // 
    public void CallInteractEvent() => HasInteracted?.Invoke();
}

public class InteractableUI
{
    private GameObject _ui;
    public GameObject UI
    {
        get { return _ui; }
        set
        {
            if(_ui != null)
            {
                _ui.SetActive(false);
            }
            _ui = value;
        }
    }
}

public class Interactable : MonoBehaviour
{
    Collider2D collider2d;
    SpriteOutline outline;
    InteractEvent interaction = new InteractEvent();
    public InteractableUI interactableUI = new InteractableUI();

    [SerializeField] LayerMask[] whatIsInteractor;

    public InteractEvent Interact
    {
        get
        {
            if (interaction == null) interaction = new InteractEvent();
            return interaction;
        }
    }
    Interactor _interactor;
    public Interactor interactor
    {
        get { return _interactor; }
    }

    public void FocusIn()
    {
        outline.UpdateOutline(true);

    }

    public void FocusOut()
    {
        outline.UpdateOutline(false);
        interactableUI.UI.SetActive(false);

    }

    public InteractableUI CallInteract(Interactor interactor)
    {
        this._interactor = interactor;
        interaction.CallInteractEvent();
        return interactableUI;
    }

    public Transform GetTransform()
    {
        return gameObject.transform;
    }
    private void Awake()
    {
        LayerMask finalMask = 0;

        foreach(var layer in whatIsInteractor)
        {
            finalMask |= layer;
        }

        TryGetComponent(out outline);
        TryGetComponent(out collider2d);
        collider2d.callbackLayers = finalMask;
    }

}
