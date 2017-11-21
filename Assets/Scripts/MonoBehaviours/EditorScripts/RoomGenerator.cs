using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


[ExecuteInEditMode]
public class RoomGenerator : MonoBehaviour
{
#if UNITY_EDITOR

    public Point gridSize;
    public float tileSize = 1;
    public GameObject tilePrefab;
    public List<Tile> tiles = new List<Tile>();


    [Button("Generate Grid")]
    [ButtonGroup("Grid Controls")]
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
                GameObject obj = Instantiate(tilePrefab, point.CalcWorldCoord(0, tileSize), Quaternion.identity);
                obj.transform.SetParent(this.transform);
                obj.name = point.ToString();

                Tile tile = new Tile(point, true, obj);

                obj.GetComponent<TileInteractable>().tile = tile;

                tiles.Add(tile);
            }
        }

        Dictionary<Point, Tile> dictionary = tiles.ToDictionary(tile => tile.point);
        tiles.ForEach(tile => tile.FindNeighbours(dictionary));
    }

    [Button("Clean up Grid")]
    [ButtonGroup("Grid Controls")]
    public void CleanUpGrid()
    {
        tiles.RemoveAll(tile => tile.obj == null);

        Dictionary<Point, Tile> dictionary = tiles.ToDictionary(tile => tile.point);
        tiles.ForEach(tile => tile.FindNeighbours(dictionary));
    }

    private void DestroyGrid()
    {
        tiles.ForEach(tile => Destroy(tile.obj));
        tiles = new List<Tile>();
    }

#endif
}

