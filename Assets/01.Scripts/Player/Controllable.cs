using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controllable : MonoBehaviour
{
    bool inputLocked = false;

    Component currentLocker;

    public bool Lock(Component requester)
    {
        if(currentLocker == null)
        {
            currentLocker = requester;
            inputLocked = true;
            UpdateInput();
            return true;
        }

        return false;
    }

    public bool UnLock(Component requester)
    {
        if(currentLocker == requester)
        {
            currentLocker = null;
            inputLocked = false;
            UpdateInput();
            return true;
        }

        return false;
    }

    public bool InputLocked { get { return inputLocked; } }

    [Header("Input Command")]
    bool inputInteract = false;
    public bool InputInteract {  get { UpdateInput(); return inputInteract; } }

    Vector2 axis;
    public Vector2 Axis { get { UpdateInput(); return axis; } }
    Vector2 axis_Abs;
    public Vector2 Axis_Abs { get { UpdateInput(); return axis_Abs; } }

    bool inputJump = false;
    public bool InputJump { get { UpdateInput(); return inputJump; } }

    bool inputR = false;
    public bool InputR { get { UpdateInput(); return inputR; } }

    bool inputEsc = false;
    public bool InputEsc { get { UpdateInput(); return inputEsc; } }


    bool updatedThisFrame = false;


    public void UpdateInput()
    {
        if(updatedThisFrame == false)
        {
            axis.x = inputLocked ? 0 : Input.GetAxisRaw("Horizontal") ;
            axis.y = inputLocked ? 0 : Input.GetAxisRaw("Vertical");

            axis_Abs.x = Mathf.Abs(axis.x);
            axis_Abs.y = Mathf.Abs(axis.y);

            inputJump = inputLocked ? false : Input.GetButtonDown("Jump");
            inputInteract = inputLocked ? false : Input.GetKeyDown(KeyCode.E);
            inputR = inputLocked ? false : Input.GetKeyDown(KeyCode.R);
            inputEsc = inputLocked ? false : Input.GetKeyDown(KeyCode.Escape);
            updatedThisFrame = true;
        }
    }

    private void LateUpdate()
    {
        updatedThisFrame = false;
    }
}
