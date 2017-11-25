using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


[ExecuteInEditMode]
public class GridGenerator : MonoBehaviour
{
#if UNITY_EDITOR

    [BoxGroup("Configuration")]
    public Point gridSize;

    [BoxGroup("Configuration")]
    public float tileSize = 1;

    [BoxGroup("Configuration")]
    public GameObject tilePrefab;

    [BoxGroup("State Management")]
    public SceneStateController stateController;

    [BoxGroup("Grid Management")]
    public List<Tile> tiles = new List<Tile>();


    [BoxGroup("Grid Management")]
    [ButtonGroup("Grid Management/Controls")]
    [Button("Generate Grid", ButtonSizes.Medium)]
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

                Tile tile = Tile.CreateInstance(point, true, obj);

                obj.GetComponent<TileInteractable>().tile = tile;

                tiles.Add(tile);
            }
        }

        Dictionary<Point, Tile> dictionary = tiles.ToDictionary(tile => tile.point);
        tiles.ForEach(tile => tile.FindNeighbours(dictionary));
    }


    [BoxGroup("Grid Management")]
    [ButtonGroup("Grid Management/Controls")]
    [Button("Clean up Grid", ButtonSizes.Medium)]
    public void CleanUpGrid()
    {
        tiles.RemoveAll(tile => tile.obj == null);

        Dictionary<Point, Tile> dictionary = tiles.ToDictionary(tile => tile.point);
        tiles.ForEach(tile => tile.FindNeighbours(dictionary));
    }


    [BoxGroup("State Management")]
    [Button("Copy Grid to State", ButtonSizes.Medium)]
    public void CopyGridToState()
    {
        this.stateController.tiles = this.tiles;
    }


    private void DestroyGrid()
    {
        tiles.ForEach(tile => Destroy(tile.obj));
        tiles = new List<Tile>();
    }

#endif
}

