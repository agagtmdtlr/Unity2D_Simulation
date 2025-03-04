using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class Placeable : MonoBehaviour
{
    public Vector2Int Size;
    public TileBase buildTile;
    public List<BoundsInt> bounds = new List<BoundsInt>();

    public Vector3Int LeftBottom { get { return BuildingHandler.Instance.gridLayout.WorldToCell(transform.position); } }
    public Vector3Int RightTop { get { return LeftBottom + new Vector3Int(Size.x ,Size.y,0); } }
   

    private void CalculateSizeInCells()
    {
        BoxCollider2D b = gameObject.GetComponent<BoxCollider2D>();
        var sizef = b.bounds.extents * 2f;
        sizef += Vector3.one * 0.5f;
        Size = new Vector2Int((int)sizef.x, (int)sizef.y);

        int cnt = transform.childCount;
        for(int i = 0; i < cnt; i++)
        {
            var c = transform.GetChild(i);
            if(c.CompareTag("Build") && c.TryGetComponent(out BoxCollider2D cbox))
            {

                BoundsInt bound = new BoundsInt();
                bound.min = BuildingHandler.Instance.gridLayout.WorldToCell(cbox.bounds.min - transform.position);
                bound.max = BuildingHandler.Instance.gridLayout.WorldToCell(cbox.bounds.max - transform.position);

                bounds.Add( bound );
            }
        }
    }

    void Start()
    {
        CalculateSizeInCells();


        Vector3Int cp = BuildingHandler.Instance.gridLayout.WorldToCell(transform.position);
        transform.position = BuildingHandler.Instance.gridLayout.CellToWorld(cp);

        var ladderRender = transform.GetChild(0).GetComponent<SpriteRenderer>();
        ladderRender.size = new Vector2 (ladderRender.size.x , Size.y + 0.5f);
        Place();
    }

    public void ChangeColor(Color color)
    {
        SpriteRenderer[] renderers = GetComponents<SpriteRenderer>();
        foreach (var r in renderers)
        {
            r.color = color;
        }
    }

    public bool IsCollapseDetail(Placeable other , Vector3Int pos , Vector3Int otherPos)
    {
        foreach (var otherb in other.bounds)
        {
            var omin = otherPos + otherb.min;
            var omax = otherPos + otherb.max;

            foreach (var b in bounds)
            {
                var min = pos + b.min;
                var max = pos + b.max;
                if (max.x > omin.x && omax.x > min.x && max.y > omin.y && omax.y > min.y)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsCollapse(Placeable other)
    {
        if (other == this)
            return false;




       

        Vector3Int pos = BuildingHandler.Instance.gridLayout.WorldToCell(transform.position);
        Vector3Int endPos = pos;
        endPos.x += Size.x;
        endPos.y += Size.y;

        Vector3Int otherPos = BuildingHandler.Instance.gridLayout.WorldToCell(other.transform.position);
        Vector3Int otherEndPos= otherPos;
        otherEndPos.x += other.Size.x;
        otherEndPos.y += other.Size.y;

        if (endPos.x > otherPos.x && otherEndPos.x > pos.x && endPos.y > otherPos.y && otherEndPos.y > pos.y )
        {
            return IsCollapseDetail(other, pos, otherPos);
        }

        return false;

    }

    public bool IsInner(Vector3Int pos)
    {

        Vector3Int cp = BuildingHandler.Instance.gridLayout.WorldToCell(transform.position);
        Vector3Int ep = cp;
        ep.x += Size.x;
        ep.y += Size.y;

        if (pos.x <= cp.x || ep.x <= pos.x)
            return false;

        if (pos.y <= cp.y || ep.y <= pos.y)
            return false;

        return true;
    }


    public void Place()
    {
        Vector3Int cp = BuildingHandler.Instance.gridLayout.WorldToCell(transform.position);
        var wt = BuildingHandler.Instance.fillTile;
        var lt = BuildingHandler.Instance.lineTile;

        for (int i = 0;  i < Size.x; i++)
        {
            for(int j = 0; j < Size.y; j++)
            {
                BuildingHandler.Instance.overlayMap.SetTile(new Vector3Int(cp.x + i , cp.y + j, cp.z), wt);
                BuildingHandler.Instance.lineOverlayMap.SetTile(new Vector3Int(cp.x + i , cp.y + j, cp.z), lt);
            }
        }
    }

    public void DePlace()
    {

    }
}
