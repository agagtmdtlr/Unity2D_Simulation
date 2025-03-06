using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UISpawner : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject alertPrefab;


    GameObject alertContainer = null;

    static private UISpawner instance = null;
    static public UISpawner Instance {  get { return instance; } }

    GameObject SpawnAlertUI_instance()
    {
        GameObject alertUI = null;

        if (instance.alertContainer is null)
        {
            alertUI = new GameObject("alert_container");
            alertUI.transform.SetParent(instance.canvas.transform);
            alertUI.transform.position = Vector3.zero;
            alertUI.transform.localScale = Vector3.one;
        }

        alertUI = Instantiate(alertPrefab, instance.alertContainer.transform);

        return alertUI;
    }

    static public GameObject SpawnAlertUI()
    {
        return instance.SpawnAlertUI_instance();
    }
    
}
