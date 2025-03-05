using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class Placeable : MonoBehaviour
{
    public Vector3Int Size;
    public List<BoundsInt> bounds = new List<BoundsInt>();
    public Vector3Int LeftBottom { get { return BuildingSystem.Instance.gridLayout.WorldToCell(transform.position); } }
    public Vector3Int RightTop { get { return LeftBottom + new Vector3Int(Size.x ,Size.y,0); } }

    public List<Transform> bridges = new List<Transform>();
    public List<SpriteRenderer> renderers = new List<SpriteRenderer>();
    public List<PlatformGenerator> platformGenerators = new List<PlatformGenerator>();

    public GameObject ladderBottom;


    public void ExtendLadderToGround(LayerMask layerMask)
    {

        Vector3Int beginInt = LeftBottom;
        beginInt.x += Size.x / 3;

        Vector3 origin = BuildingSystem.Instance.gridLayout.CellToWorld(beginInt);

        // raycast 검사로 바닥까지의 거리를 계산한다.
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 1000f, layerMask);
        if (hit.collider != null)
        {
            // 사다리는 아래에 처음 만나는 바닥까지 생성한다.
            ladderBottom.SetActive(true);
            var ladderSize = ladderBottom.GetComponent<SpriteRenderer>().size;
            ladderSize.y = hit.distance;
            ladderBottom.GetComponent<SpriteRenderer>().size = ladderSize;

            ladderBottom.transform.position = origin + Vector3.down * hit.distance;
        }
        else
        {
            ladderBottom.SetActive(false);
        }
    }
    public void ExtendBridgeToGround(LayerMask layer)
    {
        foreach (var bridge in bridges)
        {
            Vector3 bp = bridge.position;
            RaycastHit2D bhit = Physics2D.Raycast(bp, Vector2.down, 1000f, layer);
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
    public void GenerateTileColliderFromBound()
    {
        foreach (var pg in platformGenerators)
            pg.GenerateTileCollider();
    }

    private void CalculateSizeInCells()
    {
        BoxCollider2D b = gameObject.GetComponent<BoxCollider2D>();
        var sizef = b.bounds.extents * 2f;
        sizef += Vector3.one * 0.5f;
        Size = new Vector3Int((int)sizef.x, (int)sizef.y, 0);

        int cnt = transform.childCount;
        for(int i = 0; i < cnt; i++)
        {
            var c = transform.GetChild(i);
            if(c.CompareTag("Build") && c.TryGetComponent(out BoxCollider2D cbox))
            {

                BoundsInt bound = new BoundsInt();
                bound.min = BuildingSystem.Instance.gridLayout.WorldToCell(cbox.bounds.min - transform.position);
                bound.max = BuildingSystem.Instance.gridLayout.WorldToCell(cbox.bounds.max - transform.position);

                bounds.Add( bound );
            }

            if(c.CompareTag("Bridge"))
            {
                bridges.Add(c);
            }

            if(c.TryGetComponent(out PlatformGenerator pg))
            {
                platformGenerators.Add(pg);
            }

            if(c.TryGetComponent(out SpriteRenderer r))
            {
                renderers.Add(r);
            }
            renderers.AddRange(c.GetComponentsInChildren<SpriteRenderer>());
        }
        renderers.Add(GetComponent<SpriteRenderer>());

    }

    void Start()
    {
        //transform.position = BuildingSystem.Instance.SnapCoordinateToGrid(transform.position);
        CalculateSizeInCells();

        var ladderRender = transform.GetChild(0).GetComponent<SpriteRenderer>();
        ladderRender.size = new Vector2 (ladderRender.size.x , Size.y + 0.5f);

        ladderBottom = transform.Find("LadderBottom").gameObject;
    }

    public void ChangeColor(Color color)
    {
        //SpriteRenderer[] renderers = GetComponents<SpriteRenderer>();
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

        Vector3Int pos = BuildingSystem.Instance.gridLayout.WorldToCell(transform.position);
        Vector3Int endPos = pos;
        endPos.x += Size.x;
        endPos.y += Size.y;

        Vector3Int otherPos = BuildingSystem.Instance.gridLayout.WorldToCell(other.transform.position);
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

        Vector3Int cp = BuildingSystem.Instance.gridLayout.WorldToCell(transform.position);
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
        /*Vector3Int cp = BuildingSystem.Instance.gridLayout.WorldToCell(transform.position);
        var wt = BuildingSystem.Instance.fillTile;
        var lt = BuildingSystem.Instance.lineTile;

        for (int i = 0;  i < Size.x; i++)
        {
            for(int j = 0; j < Size.y; j++)
            {
                BuildingSystem.Instance.overlayMap.SetTile(new Vector3Int(cp.x + i , cp.y + j, cp.z), wt);
                BuildingSystem.Instance.lineOverlayMap.SetTile(new Vector3Int(cp.x + i , cp.y + j, cp.z), lt);
            }
        }*/
    }

    public void DePlace()
    {

    }
}
