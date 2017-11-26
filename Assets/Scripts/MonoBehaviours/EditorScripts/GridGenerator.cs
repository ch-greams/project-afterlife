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
    public SceneStateController stateCtrl;

    [BoxGroup("Grid Management")]
    public List<Tile> tiles = new List<Tile>();


    [BoxGroup("Grid Management")]
    [ButtonGroup("Grid Management/Controls")]
    [Button("Generate Grid", ButtonSizes.Medium)]
    public void CreateGrid()
    {
        this.transform.position = new Vector3();
        this.DestroyGrid();

        Debug.Log(string.Format("Generating Grid with size {0}", this.gridSize));

        for (int x = 0; x < this.gridSize.x; x++)
        {
            for (int y = 0; y < this.gridSize.y; y++)
            {
                Point point = new Point(x, y);
                GameObject obj = Instantiate(this.tilePrefab, point.CalcWorldCoord(0, tileSize), Quaternion.identity);
                obj.transform.SetParent(this.transform);
                obj.name = point.ToString();

                Tile tile = Tile.CreateInstance(point, true, obj);
                obj.GetComponent<TileInteractable>().tile = tile;

                this.tiles.Add(tile);
            }
        }

        GridGenerator.SetNeighbours(this.tiles);
    }


    [BoxGroup("Grid Management")]
    [ButtonGroup("Grid Management/Controls")]
    [Button("Clean up Grid", ButtonSizes.Medium)]
    public void CleanUpGrid()
    {
        this.tiles.RemoveAll(tile => tile.obj == null);
        GridGenerator.SetNeighbours(this.tiles);
    }


    [BoxGroup("State Management")]
    [Button("Copy Grid to State", ButtonSizes.Medium)]
    public void CopyGridToState()
    {
        this.stateCtrl.tiles = this.tiles;
    }

    private void DestroyGrid()
    {
        this.tiles.ForEach(tile => Destroy(tile.obj));
        this.tiles = new List<Tile>();
    }

    private static void SetNeighbours(List<Tile> tiles)
    {
        Dictionary<Point, Tile> dictionary = tiles.ToDictionary(tile => tile.point);
        tiles.ForEach(tile => tile.FindNeighbours(dictionary));
    }

#endif
}

