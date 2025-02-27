using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IToggle
{
    public void FocusIn();
    public void FocusOut();
}

public class IToggleDefault : IToggle
{
    public void FocusIn() { }
    public void FocusOut() { }

}

