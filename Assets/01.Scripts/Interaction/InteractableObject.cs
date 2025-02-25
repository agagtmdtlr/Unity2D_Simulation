using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractExecutor
{

}

public class InteractableObject : MonoBehaviour
{
    SpriteRenderer render;
    SpriteOutline outline;
    bool InputInteract;

    protected virtual void OnInteract(GameObject executer)
    {
    }

    protected virtual void Start()
    {
        TryGetComponent(out render);
        TryGetComponent(out outline);

    }

    protected virtual void OnStayInteractor()
    {

    }

    protected virtual void OnExitInteractor()
    {
    }

    private void FixedUpdate()
    {
        InputInteract = Input.GetKey(KeyCode.E);
    }

    protected virtual void Update()
    {
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        outline.UpdateOutline(true);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 상호작용 활성화
        if(InputInteract)
        {
            OnInteract(collision.transform.gameObject);
        }
        OnStayInteractor();
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        outline.UpdateOutline(false);
        OnExitInteractor();
    }

}
