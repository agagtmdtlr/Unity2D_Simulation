using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    public GameObject ceilprefab;
    public GameObject floorprefab;
    public Vector3Int Size;
    void CalculateSizeInCell()
    {
        BoxCollider2D b = gameObject.GetComponent<BoxCollider2D>();
        var sizef = b.bounds.extents * 2f;
        //sizef += Vector3.one * 0.5f;
        Size = new Vector3Int((int)sizef.x, (int)sizef.y, 0);
    }

    private void Start()
    {
        CalculateSizeInCell();
        GenerateTileCollider();
    }
    public void GenerateTileCollider()
    {
        var grid = BuildingSystem.Instance.gridLayout;
        var floormap = BuildingSystem.Instance.floorMap;
        var ceilmap = BuildingSystem.Instance.ceilMap;

        var box = GetComponent<BoxCollider2D>();
        var min = BuildingSystem.WorldToGrid( box.bounds.min );
        var max = BuildingSystem.WorldToGrid( box.bounds.max );
        
        for(int i = min.x; i < max.x; i++)
        {
            BuildingSystem.Instance.SetTileToFloor(new Vector3Int( i, min.y -1, 0));
            BuildingSystem.Instance.SetTileToCeil(new Vector3Int( i, max.y - 1, 0));
        }
    }

    public void UpdateCollider()
    {
        var box = GetComponent<BoxCollider2D>();
        float snappedSize = Mathf.Round(box.bounds.extents.y * 2f);
        float snappedExtend = snappedSize * 0.5f;
        var centerOffset = box.bounds.center - transform.position;

        {
            GameObject ceil = Instantiate(ceilprefab, transform);
            ceil.transform.localScale = Vector3.one;

            var cb = ceil.GetComponent<BoxCollider2D>();
            var sizeTo = (box.size);
            sizeTo.y = cb.size.y;
            cb.size = sizeTo;

            
            // bottom pivot에 맞추기 위해서 -0.5f
            var moveTo = (new Vector3( 0f, snappedExtend - 0.5f, 0));
            moveTo += centerOffset;
            ceil.transform.localPosition = moveTo;
        }
        {
            GameObject floor = Instantiate(floorprefab, transform);
            floor.transform.localScale = Vector3.one;

            var fb = floor.GetComponent<BoxCollider2D>();
            var sizeTo = (box.size);
            sizeTo.y = fb.size.y;
            fb.size = sizeTo;

            var moveTo = (new Vector3(0f, -snappedExtend - 0.5f, 0));
            moveTo += centerOffset;
            floor.transform.localPosition = moveTo;
        }

    }

    
}
