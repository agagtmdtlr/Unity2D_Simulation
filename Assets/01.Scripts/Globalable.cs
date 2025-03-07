using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Globalable<T> : MonoBehaviour
    where T : class    
{
    static private T instance = null;
    static public T Instance { 
        get
        { 
            if(instance == null)
            {
                Debug.LogAssertion("Should Access Global instance Since Start Event");
            }
            return instance; 
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this as T;
            Awake_internal();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected abstract void Awake_internal();

}
