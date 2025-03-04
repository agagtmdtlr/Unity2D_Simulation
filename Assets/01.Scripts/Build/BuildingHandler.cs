using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Linq;



public class BuildingHandler : MonoBehaviour
{
    static private BuildingHandler instance;
    static public BuildingHandler Instance
    {
        get { return instance; }
    }

    public GridLayout gridLayout;

    Grid grid;
    public Tilemap buildmap;


    public Tilemap overlayMap;
    public TileBase fillTile;

    public Tilemap lineOverlayMap;
    public TileBase lineTile;

    public Vector3Int beginPos;
    public Vector3Int endPos;

    public GameObject pointer;
    public GameObject ladderBottom;

    Vector3 cellSize { get { return buildmap.cellSize; } }

    BuildState.Mode currentState = BuildState.Mode.None;

    public GameObject currentPrefab;
    [HideInInspector] public bool selected = false;
    [HideInInspector] public Placeable selectedPlacement;
    [HideInInspector] public List<Placeable> buildings;
    [HideInInspector] public GameObject buildModeUI;


    UI_ConstructMode_Handler contruct_ui;

    public GameObject CreateCurrentPrefabInstance(Vector3Int pos)
    {
        Vector3 worldPos = gridLayout.CellToWorld(pos);
        GameObject  prefabinstance = Instantiate(currentPrefab, worldPos, Quaternion.identity);
        if( prefabinstance.TryGetComponent(out Placeable p))
        {
            buildings.Add(p);
        }
        else
        {
            Debug.LogAssertion("Placeable 컴포넌트가 누락되어 있습니다. 반드시 포함해야 합니다");
        }
        return prefabinstance;
    }

    public void ReserveUpdateProceduralLadder()
    {
        StopCoroutine("UpdateAllProceduralLadder");
        StartCoroutine("UpdateAllProceduralLadder");
    }

    public IEnumerator UpdateAllProceduralLadder()
    {
        // collider 정보가 업데이트 된 후 수행되어야 한다.
        yield return new WaitForFixedUpdate();
        for (int i = 0; i < buildings.Count; i++)
        {
            UpdateProceduralLadder(buildings[i]);
        }
    }

    
    public void UpdateProceduralLadder(Placeable building)
    {
        Vector3Int leftbottom = building.LeftBottom;
        Vector3Int rightTop = building.RightTop;

        GameObject ladderBottom = building.transform.Find("LadderBottom").gameObject;

        // 겹쳐있는 건물은 업데이트를 수행하지 않는다.
        if(collapsedObjects.Contains(building))
        {
            ladderBottom.SetActive(false);
            return;
        }

        // 바닥에 밀착된 건물이 있는지 확인한다.
        bool isConnectedFloor = false;
        if (leftbottom.y != 0) // 맨아래 바닥은 필요 없다.
        {
            foreach (var b in buildings)
            {
                var lb = b.LeftBottom;
                var rt = b.RightTop;
                if (rt.y == leftbottom.y &&
                    rt.x > leftbottom.x &&
                    rightTop.x > lb.x)
                {
                    isConnectedFloor = true;
                    break;
                }
            }
        }
        else
        {
            ladderBottom.SetActive(false);
            return;
        }

        // 바닥에 밀착된 건물이 없다면 바닥에 아래로 연결된 사다리를 생성한다.
        if (isConnectedFloor == false)
        {
            Vector3Int beginInt = leftbottom;
            beginInt.x += building.Size.x / 3;

            Vector3 origin = gridLayout.CellToWorld(beginInt);

            // raycast 검사로 바닥까지의 거리를 계산한다.
            RaycastHit2D hit =  Physics2D.Raycast(origin, Vector2.down, 1000f, 1 << 12);
            if(hit.collider != null)
            {
            }
            // 사다리는 아래에 처음 만나는 바닥까지 생성한다.
            Vector3 ladderPos = new Vector3(origin.x , hit.collider.bounds.max.y,0);
            ladderBottom.SetActive(true);
            var ladderSize = ladderBottom.GetComponent<SpriteRenderer>().size;
            ladderSize.y = Mathf.Abs(ladderPos.y - origin.y);
            ladderBottom.GetComponent<SpriteRenderer>().size = ladderSize;


            ladderBottom.transform.position = origin + Vector3.down * hit.distance;
            ladderBottom.transform.position = ladderPos;
        }
        else
        {
            ladderBottom.SetActive(false);
        }
    }

    public void DestroyPlacementInstance(Placeable placement)
    {
        buildings.Remove(placement);
        Destroy(placement.gameObject);
    }


    Dictionary<BuildState.Mode, BuildState> modeContainer = new Dictionary<BuildState.Mode, BuildState>();


    public Color unselectColor = Color.white;
    public Color selectColor = Color.green;
    public Color collapseColor = Color.blue;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        grid = gridLayout.gameObject.GetComponent<Grid>();

        buildings = new List<Placeable>();
        buildings = FindObjectsOfType<Placeable>().ToList();

        modeContainer[BuildState.Mode.Construct] = new BuildConstructState(this);
        modeContainer[BuildState.Mode.Edit] = new BuildEditState(this);
        modeContainer[BuildState.Mode.None] = new BuildNoneState(this);
        modeContainer[BuildState.Mode.ConstructMenu] = new BuildConstructMenuState(this);

    }

    private void Start()
    {
        pointer.transform.position = gridLayout.CellToWorld(Vector3Int.zero);
        ChangeMode(BuildState.Mode.Edit);

        
    }

    private void FixedUpdate()
    {

        bool isEditing = currentState.Equals(BuildState.Mode.Edit);
        isEditing |= currentState.Equals(BuildState.Mode.Construct);

    }

    private void Update()
    {
        
        modeContainer[currentState].Update();
    }

    
    public bool isCollapsed = false;
    public List<Placeable> collapsedObjects = new List<Placeable>();


    void Construct(Vector3Int gridPos, GameObject prefab)
    {
        Vector3 worldPos = gridLayout.CellToWorld(gridPos);
        Instantiate(prefab, worldPos, Quaternion.identity);
    }

    void SetActiveAllMovableCharacter(bool activate)
    {
    }

    public void ChangeMode(BuildState.Mode mode)
    {
        modeContainer[currentState].EndMode();
        currentState = mode;
        modeContainer[currentState].BeginMode();
    }

    public void BeingBuilding()
    {
        ChangeMode(BuildState.Mode.None);
        StartCoroutine(MovementPointer());
        StartCoroutine(SelectPointer());
    }
    public void EndBuilding()
    {

    }


    void StartConstructMode()
    {
        SetActiveAllMovableCharacter(false);
    }

    void EndConstructMode()
    {
        SetActiveAllMovableCharacter(true);
    }

    void StartEditMode()
    {
        SetActiveAllMovableCharacter(false);
    }

    void EndEditMode()
    {
        SetActiveAllMovableCharacter(true);
    }

    bool CheckInner(Vector3Int pos , Placeable placeable)
    {
        if(placeable.IsInner(pos))
        {
            selected = true;
            selectedPlacement = placeable;

            SpriteRenderer[] renderers = selectedPlacement.GetComponents<SpriteRenderer>();
            foreach (var r in renderers)
            {
                r.color = selectColor;
            }
            return true;
        }

        return false;
    }

    IEnumerator SelectPointer()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.E) )
            {
                if(!selected)
                {
                    Vector3Int curPos = gridLayout.WorldToCell(pointer.transform.position);
                    // 선택한 위치에 건물이 있는지 확인한다.

                    for(int i = 0; i < buildings.Count; i++)
                    {
                        if(CheckInner(curPos, buildings[i]))
                        {
                            pointer.transform.position = buildings[i].transform.position;
                            break;
                        }
                    }

                }
                else if( selected && collapsedObjects.Count > 1) // 2개 겹침 이상은 아무것도 안함
                {

                }
                else if(selected && collapsedObjects.Count == 1) // 1개 겹침 > 편집 대상을 교체한다.
                {
                    selectedPlacement = collapsedObjects[0];
                    pointer.transform.position = selectedPlacement.transform.position;

                }
                else // 선택 해제
                {
                    selected = false;
                    selectedPlacement.ChangeColor(unselectColor);
                    selectedPlacement = null;
                }
            }

            yield return null;
        }
    }

    
    IEnumerator MovementPointer()
    {
        while(true)
        {
            

            //if(dx !=0 || dy != 0)
            if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical")) 
            {
                int dx = (int)Input.GetAxisRaw("Horizontal");
                int dy = (int)Input.GetAxisRaw("Vertical");
                Vector3Int delta = new Vector3Int(dx, dy, 0);
                Vector3Int curPos = gridLayout.WorldToCell(pointer.transform.position);
                curPos += delta;

                pointer.transform.position = gridLayout.CellToWorld(curPos);
                if(selected)
                {
                    // 선택된 건물이 있다면 건물의 위치도 옮겨준다.
                    var buildingPos = gridLayout.WorldToCell(selectedPlacement.transform.position);
                    buildingPos += delta;
                    selectedPlacement.transform.position = gridLayout.CellToWorld(buildingPos);

                    isCollapsed = false;
                    collapsedObjects.Clear();


                    for (int i = 0; i < buildings.Count; i++)
                    {
                        if(buildings[i].IsCollapse(selectedPlacement))
                        {
                            isCollapsed = true;
                            collapsedObjects.Add(buildings[i]);
                        }
                        buildings[i].ChangeColor(unselectColor);
                    }


                    if(isCollapsed)
                    {
                        Color color = collapsedObjects.Count == 1 ? collapseColor : Color.red;

                        selectedPlacement.ChangeColor(color);
                        foreach(var obj in collapsedObjects)
                        {
                            obj.ChangeColor(color);
                        }
                    }    
                    else
                    {
                        selectedPlacement.ChangeColor(selectColor);
                    }
                }

            }

            yield return null;
        }
    }

    


    void IntializeOverlayMap()
    {
        overlayMap.ClearAllTiles();
        lineOverlayMap.ClearAllTiles();

        Vector3Int Size = endPos - beginPos;
        for (int i = 0; i < Size.x; i++)
        {
            for (int j = 0; j < Size.y; j++)
            {
                Vector3Int p = beginPos + new Vector3Int(i, j, 0);
                overlayMap.SetTile(p, fillTile);
                lineOverlayMap.SetTile(p, lineTile);
            }
        }
    }


    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPos =  gridLayout.WorldToCell(position);
        position = grid.GetCellCenterWorld(cellPos);
        return position;
    }


}
