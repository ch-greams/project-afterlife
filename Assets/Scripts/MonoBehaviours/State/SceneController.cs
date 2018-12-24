using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class SceneController : SerializedMonoBehaviour
{
    [ValueDropdown("sceneNames")]
    public string id;
    
    public bool isOpenWorldScene = false;
    public GlobalController globalCtrl;
    public GlobalState globalState;
    [InlineEditor]
    public SceneState sceneState;

    [InlineEditor]
    public Player player;

    [BoxGroup("Enemy Spawn Points", order: 3), HideIf("isOpenWorldScene")]
    public List<EnemySpawnPoint> enemySpawnPoints = new List<EnemySpawnPoint>();

    [BoxGroup("Interactables", order: 4)]
    public List<Interactable> interactables = new List<Interactable>();


    [ShowInInspector, HideIf("isOpenWorldScene"), BoxGroup("Grid Management", order: 1), LabelWidth(60), HorizontalGroup("Grid Management/General", 360)]
    public Point size {
        get {
            return (
                this.tiles.Any() ? this.tiles.Select(ts => ts.point).Max() + new Point(1, 1) : new Point()
            );
        }
    }

    [ShowInInspector, HideIf("isOpenWorldScene"), BoxGroup("Grid Management", order: 1), LabelWidth(60), HorizontalGroup("Grid Management/General")]
    public int total { get { return this.tiles.Count; } }
    [ShowInInspector, HideIf("isOpenWorldScene"), BoxGroup("Grid Management", order: 1), LabelWidth(60), HorizontalGroup("Grid Management/States")]
    public int _blocked { get { return this.tiles.Count(ts => ts.isBlocked); } }
    [ShowInInspector, HideIf("isOpenWorldScene"), BoxGroup("Grid Management", order: 1), LabelWidth(60), HorizontalGroup("Grid Management/States")]
    public int _visible { get { return this.tiles.Count(ts => ts.isVisible); } }
    [ShowInInspector, HideIf("isOpenWorldScene"), BoxGroup("Grid Management", order: 1), LabelWidth(60), HorizontalGroup("Grid Management/States")]
    public int _default { get { return this.tiles.Count(ts => !ts.isBlocked && !ts.isVisible); } }


    [BoxGroup("Tile Settings", order: 2), HideIf("isOpenWorldScene")]
    public Color selectedTileColor;
    [BoxGroup("Tile Settings", order: 2), HideIf("isOpenWorldScene")]
    public Color activeTileColor;
    [BoxGroup("Tile Settings", order: 2), HideIf("isOpenWorldScene")]
    public Color visibleTileColor;
    [BoxGroup("Tile Settings", order: 2), HideIf("isOpenWorldScene")]
    public Color disabledTileColor;


    [HideInInspector]
    public List<Tile> tiles = new List<Tile>();

    [HideInInspector]
    public Dictionary<string, List<Tile>> highlightedTiles = new Dictionary<string, List<Tile>>();

    
    private List<string> sceneNames { get { return GlobalController.sceneNames; } }



    private void Awake()
    {
        this.globalCtrl = GameObject.FindObjectOfType<GlobalController>();
        this.globalCtrl.sceneCtrl = this;
        
        this.globalCtrl.enemyManager.enemySpawnPoints = this.enemySpawnPoints;

        this.globalState = this.globalCtrl.globalState;
        this.sceneState = this.globalState.sceneStates[this.id];

        if (this.isOpenWorldScene)
        {
            // TODO: Replace Vector3.zero with something else
            this.player.InitPlayer(this.globalCtrl, Vector3.zero);
        }
        else
        {
            this.player.InitPlayer(
                globalCtrl: this.globalCtrl,
                tile: this.tiles.Find(t => t.point == this.globalState.currentPosition),
                visibleRange: this.globalState.currentVisibility
            );
        }
    }

    private void Start()
    {
        if (!this.sceneState.visibleByDefault)
        {
            this.UpdateTiles(this.player.tile);
        }
    }


    // NOTE: Framedrop from ~100Mb of garbage on full 20 visibility
    public void UpdateTiles(Tile playerTile)
    {
        HashSet<Point> playerTiles = playerTile.GetPointsInRange(this.player.visibleRange, (t) => (true));
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

    [Button(ButtonSizes.Medium), BoxGroup("Enemy Spawn Points", order: 3), HideIf("isOpenWorldScene")]
    public void CollectEnemySpawnPoints()
    {
        this.enemySpawnPoints = GameObject.FindObjectsOfType<EnemySpawnPoint>().ToList();
    }

    [Button(ButtonSizes.Medium), BoxGroup("Interactables", order: 4)]
    public void CollectInteractables()
    {
        this.interactables = GameObject.FindObjectsOfType<Interactable>().ToList();
    }
}
