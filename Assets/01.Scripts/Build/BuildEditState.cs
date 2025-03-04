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
        if (Input.GetKeyDown(KeyCode.Escape))
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
            // ������ ��ġ�� �ǹ��� �ִ��� Ȯ���Ѵ�.

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
        else if (selected && collapsedObjects.Count > 1) // 2�� ��ħ �̻��� �ƹ��͵� ����
        {

        }
        else if (selected && collapsedObjects.Count == 1) // 1�� ��ħ > ���� ����� ��ü�Ѵ�.
        {
            selectedObject = collapsedObjects[0];
            pointer.transform.position = selectedObject.transform.position;

            history.Add(new EditHistory(selectedObject.transform.position, selectedObject));

        }
        else // ���� ����
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
            // ���õ� �ǹ��� �ִٸ� �ǹ��� ��ġ�� �Ű��ش�.
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
                selectedObject.ChangeColor(selectColor);
            }
            context.ReserveUpdateProceduralLadder();

        }
    }
}