using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Linq;
public abstract class BuildState
{
    public enum Mode
    {
        None,
        SideMenu,
        ConstructMenu,
        Construct,
        Edit
    }

    protected BuildingSystem context;

    public Vector3Int ClampCellPos(Vector3Int pos)
    {
        pos.x = Mathf.Clamp(pos.x, context.beginPos.x, context.endPos.x);
        pos.y = Mathf.Clamp(pos.x, context.beginPos.y, context.endPos.y);
        pos.z = Mathf.Clamp(pos.z, context.beginPos.z, context.endPos.z);
        return pos;
    }
    

    public Color unselectColor { get { return context.unselectColor; } }
    public Color selectColor { get { return context.selectColor; } }
    public Color collapseColor { get { return context.collapseColor; } }

    public bool selected { get { return context.selected; } set { context.selected = value; } }
    public Placeable selectedObject { get { return context.selectedPlacement; } set { context.selectedPlacement = value; } }
    public List<Placeable> buildings { get { return context.buildings; } set { context.buildings = value; } }
    public GameObject buildModeUI { get { return context.buildModeUI; } }

    public bool isCollapsed { get { return context.isCollapsed; } set { context.isCollapsed = value; } }
    public List<Placeable> collapsedObjects { get { return context.collapsedObjects; } set { context.collapsedObjects = value; } }

    public GameObject pointer { get { return context.pointer; } }
    public GridLayout gridLayout { get { return context.gridLayout; } }


    public BuildState(BuildingSystem context)
    {
        this.context = context;
    }

    public bool CheckInner(Vector3Int pos, Placeable placeable)
    {
        if (placeable.IsInner(pos))
        {
            return true;
        }

        return false;
    }

    public abstract void Update();
    public abstract void BeginMode();
    public abstract void EndMode();
    public abstract void Check();
}

public class BuildNoneState : BuildState
{
    public BuildNoneState(BuildingSystem context) : base(context)
    {

    }

    public override void BeginMode()
    {
    }

    public override void Check()
    {
    }

    public override void EndMode()
    {
    }

    public override void Update()
    {
    }
}


public class BuildSideMenuState : BuildState
{
    bool menuSelected = false;
    Mode mode;
    public BuildSideMenuState(BuildingSystem context) : base(context)
    {
        context.sidemenuUI.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(OnSelectConstruct);
        context.sidemenuUI.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(OnSelectEdit);
    }

    public override void BeginMode()
    {
        menuSelected = false;
        mode = Mode.ConstructMenu;
        context.sidemenuUI.SetActive(true);
    }

    public override void Update()
    {
    }

    void OnSelectConstruct()
    {
        mode = Mode.ConstructMenu;
        menuSelected = true;
    }

    void OnSelectEdit()
    {
        mode = Mode.Edit;
        menuSelected = true;
    }

    public override void Check()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            context.ChangeMode(Mode.None);
            context.EndBuilding();
        }

        if (menuSelected)
        {
            context.ChangeMode(mode);
        }
    }

    public void Movement()
    {
    }

    public override void EndMode()
    {
        context.sidemenuUI.SetActive(false);
    }
}




