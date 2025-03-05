using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Linq;
using Cinemachine;


public class BuildingSystem : MonoBehaviour
{
    static private BuildingSystem instance;
    static public BuildingSystem Instance
    {
        get { return instance; }
    }

    public GridLayout gridLayout;

    Grid grid;
    public Tilemap buildmap;


    public Interactor interactor;

    public Tilemap overlayMap;
    public TileBase fillTile;

    public Tilemap lineOverlayMap;
    public TileBase lineTile;

    public Vector3Int beginPos;
    public Vector3Int endPos;

    public GameObject pointer;
    public GameObject ladderBottom;

    public GameObject constructMenuUI;


    public BuildSet[] buildSets;
    Vector3 cellSize { get { return buildmap.cellSize; } }

    BuildState.Mode currentState = BuildState.Mode.None;

    public BuildSet currentBuildSet;
    [HideInInspector] public bool selected = false;
    [HideInInspector] public Placeable selectedPlacement;
    [HideInInspector] public List<Placeable> buildings;
    [HideInInspector] public GameObject buildModeUI;

    public GameObject sidemenuUI;

    public bool isOutRangeToPlace(Vector3Int pos)
    {
        if (pos.x < beginPos.x || pos.x > endPos.x)
            return true;

        if (pos.y < beginPos.y || pos.y > endPos.y)
            return true;

        return false;
    }

    public GameObject CreateCurrentPrefabInstance(Vector3Int pos)
    {
        Vector3 worldPos = gridLayout.CellToWorld(pos);
        GameObject  prefabinstance = Instantiate(currentBuildSet.buildPrefab, worldPos, Quaternion.identity);
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

    public LayerMask whatIsGround;
    
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
            RaycastHit2D hit =  Physics2D.Raycast(origin, Vector2.down, 1000f, whatIsGround);
            if(hit.collider != null)
            {
                // 사다리는 아래에 처음 만나는 바닥까지 생성한다.
                Vector3 ladderPos = new Vector3(origin.x, hit.collider.bounds.max.y, 0);
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
        else
        {
            ladderBottom.SetActive(false);
        }

        foreach (var bridge in building.bridges)
        {

            Vector3 bp = bridge.position;
            RaycastHit2D bhit = Physics2D.Raycast(bp, Vector2.down, 1000f, whatIsGround);
            if (bhit.collider != null && bhit.distance > 0.5f)
            {
                bridge.gameObject.SetActive(true);

                var sr = bridge.GetComponent<SpriteRenderer>();
                var newSize = sr.size;
                newSize.y = bhit.distance;
                sr.size = newSize;
            }
            else
            {
                bridge.gameObject.SetActive(false);
            }
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
    }

    private void OnEnable()
    {
        modeContainer[BuildState.Mode.None] = new BuildNoneState(this);
        modeContainer[BuildState.Mode.SideMenu] = new BuildSideMenuState(this);
        modeContainer[BuildState.Mode.Construct] = new BuildConstructState(this);
        modeContainer[BuildState.Mode.Edit] = new BuildEditState(this);
        modeContainer[BuildState.Mode.ConstructMenu] = new BuildConstructMenuState(this);

        var pointerPos = beginPos + (endPos - beginPos) / 2;
        pointer.transform.position = gridLayout.CellToWorld(pointerPos);
        ChangeMode(BuildState.Mode.None);       

        Sensor sensor = GetComponent<Sensor>();
        sensor.interactEvent.AddListener(BeingBuilding);
    }

    private void OnDisable()
    {
        Sensor sensor = GetComponent<Sensor>();
        sensor.interactEvent.RemoveListener(BeingBuilding);
    }

    private void FixedUpdate()
    {

        bool isEditing = currentState.Equals(BuildState.Mode.Edit);
        isEditing |= currentState.Equals(BuildState.Mode.Construct);

    }

    private void Update()
    {
        modeContainer[currentState].Check();
        modeContainer[currentState].Update();

    }


    public bool isCollapsed = false;
    public List<Placeable> collapsedObjects = new List<Placeable>();


    void Construct(Vector3Int gridPos, GameObject prefab)
    {
        Vector3 worldPos = gridLayout.CellToWorld(gridPos);
        Instantiate(prefab, worldPos, Quaternion.identity);
    }

    public void SetActiveAllMovableCharacter(bool activate)
    {
    }

    public void ChangeMode(BuildState.Mode mode)
    {
        modeContainer[currentState].EndMode();
        currentState = mode;
        modeContainer[currentState].BeginMode();
    }

    public void FillBuildSpaceTile()
    {
        Vector3Int cp = beginPos;
        var wt = fillTile;
        var lt = lineTile;

        for (int i = cp.x; i < endPos.x; i++)
        {
            for (int j = cp.y; j < endPos.y; j++)
            {
                overlayMap.SetTile(new Vector3Int(i, j, 0), wt);
                lineOverlayMap.SetTile(new Vector3Int(i, j, 0), lt);
            }
        }
    }

    public void ClearBuildSpaceTile()
    {
        overlayMap.ClearAllTiles();
        lineOverlayMap.ClearAllTiles();
    }

    public GameObject buildCameraLaction;
    public CinemachineVirtualCamera virtualCamera;

    public void BeingBuilding()
    {
        virtualCamera.Follow = buildCameraLaction.transform;

        pointer.SetActive(true);
        interactor = GetComponent<Sensor>().interactor;
        ChangeMode(BuildState.Mode.SideMenu);

        Vector3Int cp = beginPos;
        var wt = fillTile;
        var lt = lineTile;

        for (int i = cp.x; i < endPos.x; i++)
        {
            for (int j = cp.y; j < endPos.y; j++)
            {
                overlayMap.SetTile(new Vector3Int(i, j, 0), wt);
                lineOverlayMap.SetTile(new Vector3Int(i, j, 0), lt);
            }
        }

        if (interactor.TryGetComponent(out Rigidbody2D playerRigidBody))
        {
            playerRigidBody.isKinematic = true;
            playerRigidBody.velocity = Vector2.zero;
        }
        if (interactor.TryGetComponent(out PlayerController playerController))
        {
            playerController.inputLocked = true;
        }

    }
    public void EndBuilding()
    {
        virtualCamera.Follow = interactor.transform;

        pointer.SetActive(false);
        overlayMap.ClearAllTiles();
        lineOverlayMap.ClearAllTiles();


        if (interactor.TryGetComponent(out Rigidbody2D playerRigidBody))
        {
            playerRigidBody.isKinematic = false;
        }
        if (interactor.TryGetComponent(out PlayerController playerController))
        {
            playerController.inputLocked = false;
        }
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
