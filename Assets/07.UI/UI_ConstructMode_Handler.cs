using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ConstructMode_Handler : MonoBehaviour
{
    public Text buildname;
    public GameObject build_list;
    public GameObject material_list;

    public List<Image> build_icons = new List<Image>();
    public List<Button> build_buttons = new List<Button>();
    public List<Image> material_icons = new List<Image>();
    public List<Text> material_amounts = new List<Text>();

    private void Awake()
    {
        int bcnt = build_list.transform.childCount;
        for(int i = 0; i < bcnt; i++)
        {
            var btnObj = build_list.transform.GetChild(i).GetChild(0).gameObject;
            build_icons.Add(btnObj.GetComponent<Image>());
            build_buttons.Add(btnObj.GetComponent<Button>());           
        }

        int mcnt = material_list.transform.childCount;
        for(int i = 0; i<mcnt; i++)
        {
            var icnObj = material_list.transform.GetChild(i);
            material_icons.Add(icnObj.GetComponent<Image>());
            material_amounts.Add(icnObj.GetChild(1).GetChild(0).GetComponent<Text>());
        }
    }
}
