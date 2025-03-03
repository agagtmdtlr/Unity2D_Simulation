using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingHandler : MonoBehaviour
{
    static private BuildingHandler instance;
    static public BuildingHandler Instance
    {
        get { return instance; }
    }

    public GridLayout gridLayout;

    Grid grid;
    Tilemap tilemap;
    TileBase whiteTile;
    Vector3 cellSize { get { return tilemap.cellSize; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        grid = gridLayout.gameObject.GetComponent<Grid>();

    }


    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPos =  gridLayout.WorldToCell(position);
        position = grid.GetCellCenterWorld(cellPos);
        return position;
    }

    void Update()
    {
    }
}
