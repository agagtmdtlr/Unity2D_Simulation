using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            context.ChangeMode(Mode.None);
        }
        else if (Input.GetKeyDown(KeyCode.E))
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
        context.ReserveUpdateProceduralLadder();
    }

    public override void EndMode()
    {
        if (isCollapsed)
        {
            context.DestroyPlacementInstance(selectedObject);
        }
        selectedObject = null;
        selected = false;
        context.ReserveUpdateProceduralLadder();

    }
}
