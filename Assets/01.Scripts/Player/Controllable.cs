using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controllable : MonoBehaviour
{
    [Header("Input Command")]
    bool inputLocked = false;

    public bool InputLocked { get { return inputLocked; } 
        set
        { 
            inputLocked = value;
            updatedThisFrame = false;
            UpdateInput();
        }
    }

    bool inputInteract = false;
    public bool InputInteract {  get { UpdateInput(); return inputInteract; } }

    Vector2 axis;
    public Vector2 Axis { get { UpdateInput(); return axis; } }
    Vector2 axis_Abs;
    public Vector2 Axis_Abs { get { UpdateInput(); return axis_Abs; } }

    bool inputJump = false;
    public bool InputJump { get { UpdateInput(); return inputJump; } }

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

            updatedThisFrame = true;
        }
    }

    private void LateUpdate()
    {
        updatedThisFrame = false;
    }
}
