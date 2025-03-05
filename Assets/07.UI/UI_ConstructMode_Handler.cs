using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ConstructMode_Handler : MonoBehaviour
{
    [Header("UI Mapper")]
    public Text buildname;
    public GameObject build_View;
    public GameObject material_list;

    [Header("UI Prefab")]
    public GameObject build_line_prefab;

    [HideInInspector] public List<Image> build_icons = new List<Image>();
    [HideInInspector] public List<Button> build_buttons = new List<Button>();
    [HideInInspector] public List<Image> material_icons = new List<Image>();
    [HideInInspector] public List<Text> material_amounts = new List<Text>();

    void CreateBuildUI(int count)
    {
        int needLineCount = (count-1) / 3 + 1;
        int curLineCount = build_View.transform.childCount;

        if (needLineCount > curLineCount)
        {
            int createLineCnt = needLineCount - curLineCount;
            for (int i = 0; i < createLineCnt; i++)
            {
                var line = Instantiate(build_line_prefab, build_View.transform);
            }
        }
    }

    void ClearCachedUI()
    {
        build_icons = new List<Image>();
        build_buttons = new List<Button>();
        material_icons = new List<Image>();
        material_amounts = new List<Text>();
    }

    public void UpdateUI(BuildSet[] sets)
    {
        ClearCachedUI();
        CreateBuildUI(sets.Length);

        int blinecnt = build_View.transform.childCount;
        for (int i = 0; i < blinecnt; i++)
        {
            var bline = build_View.transform.GetChild(i);
            for(int c = 0; c < 3; c++)
            {
                var btnObj = bline.transform.GetChild(c).GetChild(0).gameObject;
                build_icons.Add(btnObj.GetComponent<Image>());
                build_buttons.Add(btnObj.GetComponent<Button>());
            }
        }

        int mcnt = material_list.transform.childCount;
        for (int i = 0; i < mcnt; i++)
        {
            var icnObj = material_list.transform.GetChild(i);
            material_icons.Add(icnObj.GetComponent<Image>());
            material_amounts.Add(icnObj.GetChild(1).GetChild(0).GetComponent<Text>());
        }
    }

    private void Awake()
    {
        
    }
}
