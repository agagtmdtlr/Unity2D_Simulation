using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Placeable : MonoBehaviour
{
    public Vector2Int Size { get; private set; }
    private Vector3[] Vertices;

    private void GetColliderVertextPositionLocal()
    {
        BoxCollider2D b = gameObject.GetComponent<BoxCollider2D>();
        Vertices = new Vector3[4];

        Vector2 lb = b.bounds.min, rt = b.bounds.max;
        Vertices[0] = new Vector3(lb.x, lb.y, 0f) * 0.5f;
        Vertices[1] = new Vector3(rt.x, lb.y, 0f) * 0.5f;
        Vertices[2] = new Vector3(lb.x, rt.y, 0f) * 0.5f;
        Vertices[3] = new Vector3(rt.x, rt.y, 0f) * 0.5f;
    }

    private void CalculateSizeInCells()
    {
        Vector3Int[] vertices = new Vector3Int[Vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(Vertices[i]);
            vertices[i] = BuildingHandler.Instance.gridLayout.WorldToCell(worldPos);
        }

        Size = new Vector2Int(Mathf.Abs((vertices[0] - vertices[1]).x), Mathf.Abs((vertices[0] - vertices[3]).y));
    }

    private Vector3 GetStartPosition()
    {
        return transform.TransformPoint(Vertices[0]);
    }

    void Update()
    {
        GetColliderVertextPositionLocal();
        CalculateSizeInCells();


        Vector3Int cp = BuildingHandler.Instance.gridLayout.WorldToCell(transform.position);
        transform.position = BuildingHandler.Instance.gridLayout.CellToWorld(cp);
    }
}
