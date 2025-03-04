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
        ConstructMenu,
        Construct,
        Edit
    }

    protected BuildingHandler context;

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


    public BuildState(BuildingHandler context)
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
    Mode mode;
    public BuildNoneState(BuildingHandler context) : base(context)
    {
    }

    public override void BeginMode()
    {
        mode = Mode.Construct;
    }

    public override void Update()
    {
    }

    public override void Check()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            context.EndBuilding();
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            context.ChangeMode(mode);
        }
    }

    public void Movement()
    {
    }

    public override void EndMode()
    {
    }
}




