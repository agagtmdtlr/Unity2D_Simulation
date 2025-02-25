using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugTextManager : MonoBehaviour
{
    static private DebugTextManager instance = null;
    GUIStyle style = new GUIStyle();

    float rect_pos_x = 5f;
    float rect_pos_y = 5f;
    float w = Screen.width;
    float h = 20f;

    //출처: https://ssscool.tistory.com/582 [시작:티스토리]
    List<string> txts = new List<string>();
    int line = 0;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        style.normal.textColor = Color.white;
        style.fontSize = 25;
        h = style.fontSize + 1;
    }

    private string txt;
    [SerializeField] Text txtUI;

    private void LateUpdate()
    {
        txts.Clear();
        line = 0;
    }

    // Start is called before the first frame update
    public static void Write(string txt)
    {
        if(instance)
            instance.DrawText(txt);
    }

    private void DrawText(string txt)
    {
        GUI.Label(new Rect(rect_pos_x, rect_pos_y + h * line, w, h), txt, style);
        line++;
    }


    private void OnGUI()
    {
    }
}
