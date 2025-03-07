using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UISpawner : Globalable<UISpawner>
{
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject alertPrefab;
    [SerializeField] GameObject interactionBubblePrefab;

    Dictionary<string, GameObject> uicontainers;

    protected override void Awake_internal()
    {
        uicontainers = new Dictionary<string, GameObject>();
    }

    GameObject SpawnUI_interanl(GameObject prefab, string containername)
    {
        Debug.Log("Request Create UI");

        GameObject ui = null;
        
        GameObject container = null;
        if ( !uicontainers.ContainsKey(containername) )
        {
            container = new GameObject(containername);
            container.transform.SetParent(canvas.transform);
            container.transform.position = Vector3.zero;
            container.transform.localScale = Vector3.one;

            uicontainers[containername] = container;
        }
        container = uicontainers[containername];

        ui = Instantiate(prefab, container.transform);

        return ui;
    }


    public GameObject SpawnAlertUI()
    {
        return Instance.SpawnUI_interanl(Instance.alertPrefab, "alert_container");
    }

    public GameObject SpawnInteractionBubbleUI()
    {
        return SpawnUI_interanl(interactionBubblePrefab, "interacton_bubble_container");
    }
}
