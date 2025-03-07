using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Linq;
using Cinemachine;
using Unity.VisualScripting;


public class BuildingSystem : Globalable<BuildingSystem>
{
    [HideInInspector] public Interactor interactor;

    [Header("Tile Map")]
    public GridLayout gridLayout;

    public Tilemap floorMap;
    public Tilemap ceilMap;
    public TileBase colliderTile;

    public Tilemap overlayMap;
    public TileBase fillTile;

    public Tilemap lineOverlayMap;
    public TileBase lineTile;

    [Header("Build Space Data")]
    public Vector3Int beginPos;
    public Vector3Int endPos;

    public GameObject pointer;
    public GameObject ladderBottom;

    public LayerMask whatIsGround;

    [Header("Build UI")]
    public GameObject constructMenuUI;

    [Header("Building Asset")]
    public BuildSet[] buildSets;

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

    public void SetTileToFloor(Vector3Int pos)
    {
        floorMap.SetTile(pos, colliderTile);
    }    
    public void SetTileToCeil(Vector3Int pos)
    {
        ceilMap.SetTile(pos, colliderTile);
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

        floorMap.ClearAllTiles();
        ceilMap.ClearAllTiles();

        for (int i = 0; i < buildings.Count; i++)
        {
            UpdateProceduralLadder(buildings[i]);
        }
    }

    
    public void UpdateProceduralLadder(Placeable building)
    {
        Vector3Int leftbottom = building.LeftBottom;
        Vector3Int rightTop = building.RightTop;

        GameObject ladderBottom = building.ladderBottom;

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
        }

        // 바닥에 밀착된 건물이 없다면 바닥에 아래로 연결된 사다리를 생성한다.
        if (isConnectedFloor == false)
        {
            building.ExtendLadderToGround(whatIsGround);
        }

        building.GenerateTileColliderFromBound();
        building.ExtendBridgeToGround(whatIsGround);
        
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
    }

    private void OnDisable()
    {
        Sensor sensor = GetComponent<Sensor>();
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

    public void BeingBuilding(Sensor sensor)
    {
        virtualCamera.Follow = buildCameraLaction.transform;

        pointer.SetActive(true);
        interactor = sensor.interactor;
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
        if (interactor.TryGetComponent(out Controllable control))
        {
            control.Lock(this);
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
        if (interactor.TryGetComponent(out Controllable control))
        {
            control.UnLock(this);
        }
    }

    public static Vector3Int WorldToGrid(Vector3 position)
    {
        return Instance.gridLayout.WorldToCell(position);
    }

    public static Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPos = Instance.gridLayout.WorldToCell(position);
        position = Instance.gridLayout.CellToWorld(cellPos);
        return position;
    }


    protected override void Awake_internal()
    {
        buildings = new List<Placeable>();
        buildings = FindObjectsOfType<Placeable>().ToList();
    }
}
