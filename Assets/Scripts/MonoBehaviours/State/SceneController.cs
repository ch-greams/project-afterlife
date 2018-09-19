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
    public int _blocked { get { return this.tiles.Count(ts => ts.isBlocked); } }
    [ShowInInspector, BoxGroup("Grid Management"), LabelWidth(60), HorizontalGroup("Grid Management/States")]
    public int _visible { get { return this.tiles.Count(ts => ts.isVisible); } }
    [ShowInInspector, BoxGroup("Grid Management"), LabelWidth(60), HorizontalGroup("Grid Management/States")]
    public int _default { get { return this.tiles.Count(ts => !ts.isBlocked && !ts.isVisible); } }


    [HideInInspector]
    public List<Tile> tiles = new List<Tile>();

    [HideInInspector]
    public Dictionary<string, List<Tile>> highlightedTiles = new Dictionary<string, List<Tile>>();


    private void Awake()
    {
        this.globalCtrl = FindObjectOfType<GlobalController>();
        this.globalCtrl.sceneCtrl = this;
        
        this.globalState = this.globalCtrl.globalState;
        this.sceneState = this.globalState.sceneStates[this.id];

        Tile playerTile = this.tiles.Find(t => t.point == this.globalState.currentPosition);
        this.player.InitPlayer(this.globalCtrl, playerTile, this.globalState.currentVisibility);
    }

    private void Start()
    {
        this.InitTiles(this.player.tile, this.sceneState.visibleByDefault);
    }

    private void InitTiles(Tile playerTile, bool visibleByDefault)
    {   
        if (visibleByDefault)
        {
            Dictionary<Point, TileSimple> currentMap = this.sceneState.GetCurrentMap();

            foreach (Tile tile in this.tiles)
            {
                TileSimple tileSimple = currentMap[tile.point];
                tile.RefreshTileState(tileSimple.isVisible, tileSimple.isBlocked);
            }
        }
        else
        {
            this.UpdateTiles(playerTile);
        }
    }

    // NOTE: Framedrop from ~100Mb of garbage on full 20 visibility
    public void UpdateTiles(Tile playerTile)
    {
        HashSet<Point> playerTiles = playerTile.GetTiles(this.player.visibleRange, (t) => (true));
        Dictionary<Point, TileSimple> currentMap = this.sceneState.GetCurrentMap(playerTiles);

        foreach (Tile tile in this.tiles)
        {
            TileSimple tileSimple = currentMap[tile.point];
            tile.RefreshTileState(tileSimple.isVisible, tileSimple.isBlocked);
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
            tile.RefreshTileState(true, tile.isBlocked);
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
    Hallway_Labyrinth,
    Temple_Lair,
    Temple_Entrance,
    Temple_EntranceTunnel_Left,
    Temple_EntranceTunnel_Right,
    Temple_HallTunnel_Left,
    Temple_HallTunnel_Right,
    Forest_Entrance,
    Forest_Altar,
    Forest_CampOnTheCliff,
    Forest_TempleEntrance,
    Forest_Graveyard,
    Forest_DeepInTheForest,
    Forest_DeeperInTheForest,
    Playground,
}
