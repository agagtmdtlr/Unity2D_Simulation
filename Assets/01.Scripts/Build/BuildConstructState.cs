using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildConstructMenuState : BuildState
{
    // context 의 contruct menu ui를 활성화 한다.
    GameObject menuObj { get { return context.constructMenuUI; } }
    UI_ConstructMode_Handler ui;
    InventoryController inventory;

    public BuildConstructMenuState(BuildingSystem context) : base(context)
    {
        
    }

    public override void BeginMode()
    {
        inventory = context.interactor.GetComponent<InventoryController>();

        context.currentBuildSet = null;

        selected = false;
        selectedObject = null;

        menuObj.SetActive(true);
        context.SetActiveAllMovableCharacter(false);

        ui = menuObj.GetComponent<UI_ConstructMode_Handler>();
        for (int i = 0; i < context.buildSets.Length; i++)
        {
            BuildSet bs = context.buildSets[i];

            // lambda closure problem
            int index = i;
            ui.build_icons[i].sprite = bs.icn;
            ui.build_buttons[i].onClick.RemoveAllListeners();
            ui.build_buttons[i].onClick.AddListener(() => { OnSelectPrefab(index); });
        }

        OnSelectPrefab(0);
    }

    public void OnSelectPrefab(int selectIndex)
    {
        var build = context.buildSets[selectIndex];
        context.currentBuildSet = build;
        ui.buildname.text = build.buildname;


        for (int i = 0; i < build.buldMaterial.Count;i++)
        {
            var bm = build.buldMaterial[i];
            ui.material_icons[i].sprite = bm.itemStat.icon;
            ui.material_amounts[i].text = 
                $"{inventory.GetAmountOfItem(bm.itemStat)}/{bm.amount}";
        }

        
    }
    

    public override void Check()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            context.ChangeMode(Mode.SideMenu);
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            bool isCanBuyBuilding = true;

            if(context.currentBuildSet is null)
            {
                isCanBuyBuilding = false;
            }
            else
            {
                var build = context.currentBuildSet;

                for (int i = 0; i < build.buldMaterial.Count; i++)
                {
                    var bm = build.buldMaterial[i];

                    if (inventory.GetAmountOfItem(bm.itemStat) < bm.amount)
                    {
                        isCanBuyBuilding = false;
                        break;
                    }
                }
            }
            

            if(isCanBuyBuilding)
            {
                context.ChangeMode(Mode.Construct);
            }


        }

    }

    public override void Update()
    {

    }

    public override void EndMode()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuObj.SetActive(false);
            context.SetActiveAllMovableCharacter(true);
        }
    }

    
}


public class BuildConstructState : BuildState
{
    bool endContruct = false;
    bool consumeMaterial = false;
    InventoryController inventory;

    public BuildConstructState(BuildingSystem context) : base(context)
    {
    }

    public override void BeginMode()
    {

        inventory = context.interactor.GetComponent<InventoryController>();

        // reset trigger
        endContruct = false;
        consumeMaterial = false;

        pointer.SetActive(true);
        pointer.transform.position = gridLayout.CellToWorld(Vector3Int.zero);

        // building 인스턴스를 생성하고
        GameObject prefabinstance = context.CreateCurrentPrefabInstance(context.beginPos);
        context.selectedPlacement = prefabinstance.GetComponent<Placeable>();

        pointer.transform.position = context.gridLayout.CellToWorld(context.beginPos);

        isCollapsed = false;
        collapsedObjects.Clear();
    }

    public override void Update()
    {
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
            Movement();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isCollapsed) // 건설 확정
            {
                endContruct = true;
                consumeMaterial = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            endContruct = true;
        }
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
        var buildingPos = gridLayout.WorldToCell(selectedObject.transform.position);

        // 범위 clamp
        if (context.isOutRangeToPlace(buildingPos + delta) || 
            context.isOutRangeToPlace(buildingPos + delta + selectedObject.Size ) )
        {
            delta = Vector3Int.zero;
        }

        curPos += delta;
        buildingPos += delta;

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

        selectedObject.ChangeColor(isCollapsed ? Color.red : selectColor);
        foreach (var obj in collapsedObjects)
        {
            obj.ChangeColor(Color.red);
        }
        context.ReserveUpdateProceduralLadder();
    }

    public override void EndMode()
    {
        if (consumeMaterial)
        {
            selectedObject.ChangeColor(unselectColor);
            var bs = context.currentBuildSet;
            for(int i = 0; i < bs.buldMaterial.Count;i++)
            {
                var bm = bs.buldMaterial[i];
                inventory.SetAmountOfItem(bm.itemStat, -bm.amount);
            }
        }
        else
        {
            context.DestroyPlacementInstance(selectedObject);
            foreach (var obj in collapsedObjects)
            {
                obj.ChangeColor(unselectColor);
            }
        }

        selectedObject = null;
        selected = false;
        context.ReserveUpdateProceduralLadder();

    }
}
