using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class SceneController : SerializedMonoBehaviour
{
    [ValueDropdown("sceneNames")]
    public string id;
    
    public GlobalController globalCtrl;
    public GlobalState globalState;
    [InlineEditor]
    public SceneState sceneState;


    [InlineEditor]
    public Player player;


    [FoldoutGroup("Scene Settings/Tile Settings"), ShowIf("isDungeonScene"), ColorPalette("Selected Tile Palette")]
    public Color selectedTileColor;
    [FoldoutGroup("Scene Settings/Tile Settings"), ShowIf("isDungeonScene"), ColorPalette("Active Tile Palette")]
    public Color activeTileColor;
    [FoldoutGroup("Scene Settings/Tile Settings"), ShowIf("isDungeonScene"), ColorPalette("Visible Tile Palette")]
    public Color visibleTileColor;
    [FoldoutGroup("Scene Settings/Tile Settings"), ShowIf("isDungeonScene"), ColorPalette("Disabled Tile Palette")]
    public Color disabledTileColor;


    [FoldoutGroup("Scene Settings"), DictionaryDrawerSettings(IsReadOnly = true), ShowIf("isDungeonScene")]
    public Dictionary<string, EnemySpawnPoint> enemySpawnPoints = new Dictionary<string, EnemySpawnPoint>();
    [FoldoutGroup("Scene Settings"), DictionaryDrawerSettings(IsReadOnly = true)]
    public Dictionary<string, Interactable> interactables = new Dictionary<string, Interactable>();


    [HideInInspector]
    public List<Tile> tiles = new List<Tile>();
    [HideInInspector]
    public Dictionary<string, List<Tile>> highlightedTiles = new Dictionary<string, List<Tile>>();


    private bool isDungeonScene { get { return this.sceneState.isDungeonScene; } }
    private List<string> sceneNames { get { return GlobalController.sceneNames; } }



    private void Awake()
    {
        this.globalCtrl = GameObject.FindObjectOfType<GlobalController>();
        this.globalCtrl.SetScene(this);
        
        this.globalCtrl.enemyManager.enemySpawnPoints = this.enemySpawnPoints.Values.ToList();

        this.globalState = this.globalCtrl.globalState;
        this.sceneState = this.globalState.sceneStates[this.id];

        // NOTE: Init player in the scene
        if (this.sceneState.isDungeonScene)
        {
            this.player.InitPlayer(
                globalCtrl: this.globalCtrl,
                tile: this.tiles.Find(t => t.point == this.sceneState.currentPositionPoint),
                visibleRange: this.globalState.currentVisibility
            );
        }
        else
        {
            this.player.InitPlayer(
                globalCtrl: this.globalCtrl,
                playerPosition: this.sceneState.currentPositionVector,
                walkableAreaMask: this.sceneState.walkableAreas
            );
        }

        // NOTE: Load interactables by state
        foreach (KeyValuePair<string, InteractableState> kvp in this.sceneState.interactables)
        {
            this.interactables[kvp.Key].ToggleInteractable(kvp.Value.enabled, kvp.Value.visible);
        }

        // NOTE: Load spawn points by state
        foreach (KeyValuePair<string, EnemySpawnPointState> kvp in this.sceneState.enemySpawnPoints)
        {
            // TODO: Clone? Check later.
            this.enemySpawnPoints[kvp.Key].state = new EnemySpawnPointState(kvp.Value);
        }
    }

    private void Start()
    {
        if (this.sceneState.isDungeonScene)
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

    public Interactable GetInteractableInReach()
    {
        return (
            this.isDungeonScene
                ? this.interactables
                    .FirstOrDefault(kvp =>
                        kvp.Value.data.isInteractableActive &&
                        kvp.Value.data.reachablePoints.Contains(this.player.tile.point)
                    )
                    .Value
                : null
        );
    }


    private void CollectEnemySpawnPoints()
    {
        this.enemySpawnPoints = GameObject.FindObjectsOfType<EnemySpawnPoint>()
            .ToDictionary(esp => esp.gameObject.name, esp => esp);

        this.sceneState.enemySpawnPoints = this.enemySpawnPoints
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.state);
    }

    private void CollectInteractables()
    {        
        this.interactables = GameObject.FindObjectsOfType<Interactable>()
            .ToDictionary(interactable => interactable.name, interactable => interactable);

        this.sceneState.interactables = this.interactables.ToDictionary(
            kvp => kvp.Key, kvp => new InteractableState(
                enabled: kvp.Value.data.isInteractableActive,
                visible: (
                    kvp.Value.data.interactableObject != null
                        ? kvp.Value.data.interactableObject.activeSelf
                        : false
                )
            )
        );
    }

    [Button(ButtonSizes.Medium), FoldoutGroup("Scene Settings")]
    private void CollectSceneElements()
    {
        if (this.isDungeonScene)
        {
            this.CollectEnemySpawnPoints();
        }

        this.CollectInteractables();
    }
}
