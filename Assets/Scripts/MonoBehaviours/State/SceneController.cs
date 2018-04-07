using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class SceneController : SerializedMonoBehaviour
{
    public SceneType id;
    public GlobalController globalCtrl;
    public GlobalState globalState;
    public Player player;

    [InlineEditor]
    public SceneState sceneState;

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

    [HideInInspector]
    public Dictionary<string, List<Tile>> highlightedTiles = new Dictionary<string, List<Tile>>();


    private void Awake()
    {
        this.globalCtrl = FindObjectOfType<GlobalController>();
        this.globalState = this.globalCtrl.globalState;
        this.sceneState = this.globalState.sceneStates[this.id];

        this.player.UpdatePlayer(this.tiles.Find(t => t.point == this.globalState.currentPosition), this.globalState.currentVisibility);

        this.UpdateTiles(this.player.tile);
    }

    // NOTE: This probably should be optimized
    public void UpdateTiles(Tile tile)
    {
        List<Tile> visibleTiles = tile.GetTiles(this.player.visibleRange, TileState.Hidden);
        List<Point> playerLayer = visibleTiles.Select(vt => vt.point).ToList();
        List<TileSimple> currentMap = this.sceneState.GetCurrentMap(playerLayer);

        foreach (Tile t in this.tiles)
        {
            TileData tileData = t.obj.GetComponent<Interactable>().data as TileData;
            tileData.RefreshTileState(currentMap.Find(ts => ts.point == t.point).state, false);
        }

        foreach (string lightSourceId in this.highlightedTiles.Keys)
        {
            this.UpdateHighlightedTiles(lightSourceId);
        }
    }

    public void UpdateHighlightedTiles(string lightSourceId)
    {
        foreach (Tile tile in this.highlightedTiles[lightSourceId])
        {
            TileData tileData = tile.obj.GetComponent<Interactable>().data as TileData;
            TileState defaultTileState = this.sceneState.defaultMap.Find(ts => ts.point == tile.point).state;
            TileState tileState = (defaultTileState == TileState.Hidden) ? TileState.Active : defaultTileState;
            tileData.RefreshTileState(tileState, false);
        }
    }
}

public enum SceneType
{
    Undefined,
    AptN1_Bedroom,
    AptN1_LivingRoom,
    Hallway,
    AptN3_LivingRoom,
    AptN0_LivingRoom,
    AptN1_Bathroom,
    AptN3_Bathroom,
    AptN3_Bedroom,
    AptN5_Bathroom,
    AptN5_Bedroom,
    AptN5_LivingRoom,
    Hallway_ElevatorEnd,
    Forest_Field,
}
