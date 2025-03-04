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


public class BuildConstructMenuState : BuildState
{
    // context 의 contruct menu ui를 활성화 한다.
    GameObject constructMenuUI;

    public BuildConstructMenuState(BuildingHandler context) : base(context)
    {
    }

    public override void BeginMode()
    {
        selected = false;
        selectedObject = null;

        constructMenuUI.SetActive(true);
    }

    public override void Check()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            context.ChangeMode(Mode.None);
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            // TODO 재료가 충분한지 검사한다.
            bool isCanBuyBuilding = false;
            if (isCanBuyBuilding == true)
            {
                context.ChangeMode(Mode.Construct);
            }
        }
    }

    public override void Update()
    {
        Movement();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
        }
        if (Input.GetKeyDown(KeyCode.E))
        {

        }

    }

    public override void EndMode()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            constructMenuUI.SetActive(false);
        }
    }

    void Movement()
    {
    }
}



public class BuildConstructState : BuildState
{
    bool endContruct = false;

    public BuildConstructState(BuildingHandler context) : base(context)
    {
    }

    public override void BeginMode()
    {
        endContruct = false;

        pointer.SetActive(true);
        pointer.transform.position = gridLayout.CellToWorld(Vector3Int.zero);

        // building 인스턴스를 생성하고
        GameObject prefabinstance = context.CreateCurrentPrefabInstance(context.beginPos);
        context.selectedPlacement = prefabinstance.GetComponent<Placeable>();

        isCollapsed = false;
        collapsedObjects.Clear();
    }

    public override void Update()
    {
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
            Movement();

        endContruct = Input.GetKeyDown(KeyCode.E) && !isCollapsed;
        endContruct |= Input.GetKeyDown(KeyCode.Escape);
    }

    public override void Check()
    {
        if (endContruct)
        {
            context.ChangeMode(Mode.ConstructMenu);
        }
    }

    public void Movement()
    {
        int dx = (int)Input.GetAxisRaw("Horizontal");
        int dy = (int)Input.GetAxisRaw("Vertical");
        Vector3Int delta = new Vector3Int(dx, dy, 0);
        Vector3Int curPos = gridLayout.WorldToCell(pointer.transform.position);
        curPos += delta;

        pointer.transform.position = gridLayout.CellToWorld(curPos);
        if (selected)
        {
            // 선택된 건물이 있다면 건물의 위치도 옮겨준다.
            var buildingPos = gridLayout.WorldToCell(selectedObject.transform.position);
            buildingPos += delta;
            selectedObject.transform.position = gridLayout.CellToWorld(buildingPos);

            isCollapsed = false;
            collapsedObjects.Clear();


            for (int i = 0; i < buildings.Count; i++)
            {
                if (buildings[i].IsCollapse(selectedObject))
                {
                    isCollapsed = true;
                    collapsedObjects.Add(buildings[i]);
                }
                buildings[i].ChangeColor(unselectColor);
            }

            selectedObject.ChangeColor(isCollapsed ? Color.red : selectColor);
            foreach (var obj in collapsedObjects)
            {
                obj.ChangeColor(Color.red);
            }
        }
    }

    public override void EndMode()
    {
        if(isCollapsed)
        {
            context.DestroyPlacementInstance(selectedObject);
        }
        selectedObject = null;
        selected = false;
    }
}
public class BuildEditState : BuildState
{
    struct EditHistory
    {
        public Vector3 pos;
        public Placeable building;

        public EditHistory(Vector3 pos, Placeable building)
        {
            this.pos = pos;
            this.building = building;
        }
    }

    List<EditHistory> history = new List<EditHistory>();

    public BuildEditState(BuildingHandler context) : base(context)
    {
    }

    public override void BeginMode()
    {
        pointer.SetActive(true);
        pointer.transform.position = gridLayout.CellToWorld(Vector3Int.zero);

        selected = false;
        selectedObject = null;
        isCollapsed = false;
        collapsedObjects.Clear();

        history.Clear();
    }

    public override void Check()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            context.ChangeMode(Mode.None);
        }
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SelectPointer();
        }

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
        {
            Movement();
        }
    }

    public override void EndMode()
    {
        pointer.SetActive(false);
        if(history.Count > 0 )
        {
            foreach(var h in history)
            {
                h.building.transform.position = h.pos;
            }
        }
        history.Clear();
    }

    void SelectPointer()
    {
        if (!selected)
        {
            Vector3Int curPos = gridLayout.WorldToCell(pointer.transform.position);
            // 선택한 위치에 건물이 있는지 확인한다.

            for (int i = 0; i < buildings.Count; i++)
            {
                if (CheckInner(curPos, buildings[i]))
                {
                    selected = true;
                    selectedObject = buildings[i];
                    selectedObject.ChangeColor(selectColor);
                    pointer.transform.position = buildings[i].transform.position;


                    history.Add(new EditHistory(buildings[i].transform.position, buildings[i]));
                    break;
                }
            }

        }
        else if (selected && collapsedObjects.Count > 1) // 2개 겹침 이상은 아무것도 안함
        {

        }
        else if (selected && collapsedObjects.Count == 1) // 1개 겹침 > 편집 대상을 교체한다.
        {
            selectedObject = collapsedObjects[0];
            pointer.transform.position = selectedObject.transform.position;

            history.Add(new EditHistory(selectedObject.transform.position, selectedObject));

        }
        else // 선택 해제
        {
            selected = false;
            selectedObject.ChangeColor(unselectColor);
            selectedObject = null;

            history.Clear();

        }
    }

    void Movement()
    {
        int dx = (int)Input.GetAxisRaw("Horizontal");
        int dy = (int)Input.GetAxisRaw("Vertical");
        Vector3Int delta = new Vector3Int(dx, dy, 0);
        Vector3Int curPos = gridLayout.WorldToCell(pointer.transform.position);
        curPos += delta;

        pointer.transform.position = gridLayout.CellToWorld(curPos);
        if (selected)
        {
            // 선택된 건물이 있다면 건물의 위치도 옮겨준다.
            var buildingPos = gridLayout.WorldToCell(selectedObject.transform.position);
            buildingPos += delta;
            selectedObject.transform.position = gridLayout.CellToWorld(buildingPos);

            isCollapsed = false;
            collapsedObjects.Clear();


            for (int i = 0; i < buildings.Count; i++)
            {
                if (buildings[i].IsCollapse(selectedObject))
                {
                    isCollapsed = true;
                    collapsedObjects.Add(buildings[i]);
                }
                buildings[i].ChangeColor(unselectColor);
            }


            if (isCollapsed)
            {
                Color color = collapsedObjects.Count == 1 ? collapseColor : Color.red;

                selectedObject.ChangeColor(color);
                foreach (var obj in collapsedObjects)
                {
                    obj.ChangeColor(color);
                }
            }
            else
            {
                context.UpdateProceduralBuilding(selectedObject);
                selectedObject.ChangeColor(selectColor);
            }
        }
    }
}
