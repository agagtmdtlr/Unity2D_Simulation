using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    SpriteRenderer render;
    SpriteOutline outline;
    bool InputInteract;
    float interactInnerTime = 0;

    public virtual void Interact(GameObject executer)
    {
    }

    public virtual GameObject GetIneractUI()
    {
        return null;
    }

    public Transform GetTransform()
    {
        return gameObject.transform;
    }

    protected virtual void Start()
    {
        TryGetComponent(out render);
        TryGetComponent(out outline);

    }

    public virtual void OnStayInteractor()
    {
        interactInnerTime = 1.0f;
    }

    public virtual void OnExitInteractor()
    {        
    }

    private void FixedUpdate()
    {
        InputInteract = Input.GetKey(KeyCode.E);
    }

    protected virtual void Update()
    {
        if(interactInnerTime > 0.0f)
        {
            interactInnerTime -= Time.deltaTime; 
        }
        outline.UpdateOutline(interactInnerTime > 0.0f);
    }
    
}
