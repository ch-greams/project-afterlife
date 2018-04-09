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
    public TileState tileState = TileState.Hidden;

    [BoxGroup("Configuration")]
    public GameObject tilePrefab;

    [BoxGroup("State Management")]
    public SceneController stateCtrl;

    [ShowInInspector, BoxGroup("Grid Management"), LabelWidth(60), HorizontalGroup("Grid Management/General", 360)]
    public Point size {
        get {
            return (
                this.tiles.Any() ? this.tiles.Select(ts => ts.point).Max() + new Point(1, 1) : new Point()
            );
        }
    }
    [ShowInInspector, BoxGroup("Grid Management"), LabelWidth(60), HorizontalGroup("Grid Management/General")]
    public int total { get { return this.tiles.Count; } }

    [ShowInInspector, BoxGroup("Grid Management"), LabelWidth(60), HorizontalGroup("Grid Management/States")]
    public int active { get { return this.tiles.Count(ts => ts.state == TileState.Active); } }
    [ShowInInspector, BoxGroup("Grid Management"), LabelWidth(60), HorizontalGroup("Grid Management/States")]
    public int hidden { get { return this.tiles.Count(ts => ts.state == TileState.Hidden); } }
    [ShowInInspector, BoxGroup("Grid Management"), LabelWidth(60), HorizontalGroup("Grid Management/States")]
    public int disabled { get { return this.tiles.Count(ts => ts.state == TileState.Disabled); } }


    [HideInInspector]
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

                Tile tile = Tile.CreateInstance(point, this.tileState, obj);
                Interactable tileInteractable = obj.GetComponent<Interactable>();
                tileInteractable.sceneCtrl = this.stateCtrl;
                (tileInteractable.data as TileData).tile = tile;

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
        this.stateCtrl.sceneState.defaultMap = this.tiles.Select(t => new TileSimple(t)).ToList();
    }

    private void DestroyGrid()
    {
        this.tiles.ForEach(tile => DestroyImmediate(tile.obj));
        this.tiles = new List<Tile>();
    }

    private static void SetNeighbours(List<Tile> tiles)
    {
        Dictionary<Point, Tile> dictionary = tiles.ToDictionary(tile => tile.point);
        tiles.ForEach(tile => tile.FindNeighbours(dictionary));
    }

#endif
}

