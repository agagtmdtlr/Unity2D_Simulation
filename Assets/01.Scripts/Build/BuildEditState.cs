using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public BuildEditState(BuildingSystem context) : base(context)
    {
    }

    public override void BeginMode()
    {
        context.SetActiveAllMovableCharacter(false);

        pointer.SetActive(true);

        var pointerPos = context.beginPos + (context.endPos - context.beginPos) / 2;

        pointer.transform.position = gridLayout.CellToWorld(pointerPos);

        selected = false;
        selectedObject = null;
        isCollapsed = false;
        collapsedObjects.Clear();

        history.Clear();
    }

    public override void Check()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            context.ChangeMode(Mode.SideMenu);
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
        context.pointer.SetActive(false);
        context.SetActiveAllMovableCharacter(true);
        pointer.SetActive(false);
        if (history.Count > 0)
        {
            foreach (var h in history)
            {
                h.building.transform.position = h.pos;
            }
        }
        history.Clear();
        context.ReserveUpdateProceduralLadder();
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

        // 선택된 건물이 있다면 위치 옮겨준다.
        if (selected)
        {

            Vector3Int curPos = gridLayout.WorldToCell(pointer.transform.position);
            var buildingPos = gridLayout.WorldToCell(selectedObject.transform.position);

            if(context.isOutRangeToPlace(buildingPos + delta) ||
                context.isOutRangeToPlace(buildingPos + delta + selectedObject.Size ))
            {
                delta = Vector3Int.zero;
            }

            buildingPos += delta;
            curPos += delta;

            pointer.transform.position = gridLayout.CellToWorld(curPos);
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
                selectedObject.ChangeColor(selectColor);
            }
            context.ReserveUpdateProceduralLadder();
        }
        else
        {
            Vector3Int curPos = gridLayout.WorldToCell(pointer.transform.position);

            if (context.isOutRangeToPlace(curPos + delta) )
            {
                delta = Vector3Int.zero;
            }
            curPos += delta;
            pointer.transform.position = gridLayout.CellToWorld(curPos);
        }
    }
}