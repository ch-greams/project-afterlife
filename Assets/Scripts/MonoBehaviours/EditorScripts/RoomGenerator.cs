using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class RoomGenerator : MonoBehaviour
{
#if UNITY_EDITOR

    public Point gridSize;
    public float tileSize = 1;
    public GameObject tilePrefab;
    public List<Tile> tiles = new List<Tile>();


    public void CreateGrid()
    {
        this.transform.position = new Vector3();
        DestroyGrid();

        Debug.Log(string.Format("Generating Grid with size {0}", gridSize));

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Point point = new Point(x, y);
                GameObject tileObj = Instantiate(tilePrefab, point.CalcWorldCoord(0, tileSize), Quaternion.identity);
                Tile tile = new Tile(point, true, tileObj, new Dictionary<Point, bool>());

                tileObj.GetComponent<TileInteractable>().tile = tile;

                tileObj.transform.SetParent(this.transform);
                tileObj.name = point.ToString();

                tiles.Add(tile);
            }
        }
    }

    public void CleanUpGrid()
    {
        tiles.RemoveAll(c => c.obj == null);
    }

    private void DestroyGrid()
    {
        foreach (Tile tile in tiles)
        {
            Destroy(tile.obj);
        }
        tiles = new List<Tile>();
    }

#endif
}

