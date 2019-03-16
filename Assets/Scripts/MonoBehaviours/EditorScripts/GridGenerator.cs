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
    public bool isVisible = false;
    [BoxGroup("Configuration")]
    public bool isBlocked = false;

    [BoxGroup("Configuration")]
    public GameObject tilePrefab;

    [BoxGroup("State Management")]
    public SceneController sceneCtrl;

    [ShowInInspector, BoxGroup("Grid Management"), LabelWidth(60), HorizontalGroup("Grid Management/General", 360)]
    public Point size {
        get {
            return (
                this.tiles.Any() ? this.tiles.Where(tile => tile).Select(tile => tile.point).Max() + new Point(1, 1) : Point.zero
            );
        }
    }

    [ShowInInspector, BoxGroup("Grid Management"), LabelWidth(60), HorizontalGroup("Grid Management/General")]
    public int total { get { return this.tiles.Count; } }
    [ShowInInspector, BoxGroup("Grid Management"), LabelWidth(60), HorizontalGroup("Grid Management/States")]
    public int _blocked { get { return this.tiles.Count(tile => tile && tile.isBlocked); } }
    [ShowInInspector, BoxGroup("Grid Management"), LabelWidth(60), HorizontalGroup("Grid Management/States")]
    public int _visible { get { return this.tiles.Count(tile => tile && tile.isVisible); } }
    [ShowInInspector, BoxGroup("Grid Management"), LabelWidth(60), HorizontalGroup("Grid Management/States")]
    public int _default { get { return this.tiles.Count(tile => tile && !tile.isBlocked && !tile.isVisible); } }



    [HideInInspector]
    public List<Tile> tiles = new List<Tile>();


    [BoxGroup("Grid Management")]
    [ButtonGroup("Grid Management/Controls")]
    [Button("Generate Grid", ButtonSizes.Medium)]
    public void CreateGrid()
    {
        this.transform.position = new Vector3(0, 0.01F, 0);
        this.DestroyGrid();

        Debug.Log(string.Format("Generating Grid with size {0}", this.gridSize));

        for (int x = 0; x < this.gridSize.x; x++)
        {
            for (int y = 0; y < this.gridSize.y; y++)
            {
                Point point = new Point(x, y);
                GameObject gameObject = GameObject.Instantiate(
                    original: this.tilePrefab, 
                    position: point.CalcWorldCoord(
                        // TODO: Validate that it works correctly (if not try +0.5F for x and z)
                        new Vector3(this.sceneCtrl.transform.position.x, 0, this.sceneCtrl.transform.position.z)
                    ), 
                    rotation: this.tilePrefab.transform.rotation
                );
                gameObject.transform.SetParent(this.transform);
                gameObject.name = point.ToString();

                this.tiles.Add(
                    gameObject.GetComponent<Tile>().Init(point, this.isVisible, this.isBlocked, this.sceneCtrl)
                );
            }
        }

        GridGenerator.SetNeighbours(this.tiles);
    }


    [BoxGroup("Grid Management")]
    [ButtonGroup("Grid Management/Controls")]
    [Button("Clean up Grid", ButtonSizes.Medium)]
    public void CleanUpGrid()
    {
        this.tiles.RemoveAll(tile => !tile);
        GridGenerator.SetNeighbours(this.tiles);
    }


    [BoxGroup("State Management")]
    [Button("Copy Grid to State", ButtonSizes.Medium)]
    public void CopyGridToState()
    {
        this.sceneCtrl.tiles = this.tiles;
        this.sceneCtrl.sceneState.defaultMap = this.tiles.Select(t => new TileSimple(t)).ToList();
    }

    private void DestroyGrid()
    {
        this.tiles.ForEach(tile => DestroyImmediate(tile.gameObject));
        this.tiles = new List<Tile>();
    }

    private static void SetNeighbours(List<Tile> tiles)
    {
        Dictionary<Point, Tile> dictionary = tiles.ToDictionary(tile => tile.point);
        tiles.ForEach(tile => tile.FindNeighbours(dictionary));
    }

#endif
}

