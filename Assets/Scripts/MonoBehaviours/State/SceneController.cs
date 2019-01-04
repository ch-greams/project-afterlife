﻿using System.Linq;
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

    [BoxGroup("Enemy Spawn Points", order: 3), ShowIf("isDungeonScene")]
    public List<EnemySpawnPoint> enemySpawnPoints = new List<EnemySpawnPoint>();

    [BoxGroup("Interactables", order: 4)]
    public List<Interactable> interactables = new List<Interactable>();


    [ShowInInspector, ShowIf("isDungeonScene"), BoxGroup("Grid Management", order: 1), LabelWidth(60), HorizontalGroup("Grid Management/General", 360)]
    public Point size {
        get {
            return (
                this.tiles.Any() ? this.tiles.Select(ts => (ts != null) ? ts.point : Point.zero).Max() + new Point(1, 1) : Point.zero
            );
        }
    }

    [ShowInInspector, ShowIf("isDungeonScene"), BoxGroup("Grid Management", order: 1), LabelWidth(60), HorizontalGroup("Grid Management/General")]
    public int total { get { return this.tiles.Count; } }
    [ShowInInspector, ShowIf("isDungeonScene"), BoxGroup("Grid Management", order: 1), LabelWidth(60), HorizontalGroup("Grid Management/States")]
    public int _blocked { get { return this.tiles.Count(ts => (ts != null) && ts.isBlocked); } }
    [ShowInInspector, ShowIf("isDungeonScene"), BoxGroup("Grid Management", order: 1), LabelWidth(60), HorizontalGroup("Grid Management/States")]
    public int _visible { get { return this.tiles.Count(ts => (ts != null) && ts.isVisible); } }
    [ShowInInspector, ShowIf("isDungeonScene"), BoxGroup("Grid Management", order: 1), LabelWidth(60), HorizontalGroup("Grid Management/States")]
    public int _default { get { return this.tiles.Count(ts => (ts != null) && !ts.isBlocked && !ts.isVisible); } }


    [BoxGroup("Tile Settings", order: 2), ShowIf("isDungeonScene")]
    public Color selectedTileColor;
    [BoxGroup("Tile Settings", order: 2), ShowIf("isDungeonScene")]
    public Color activeTileColor;
    [BoxGroup("Tile Settings", order: 2), ShowIf("isDungeonScene")]
    public Color visibleTileColor;
    [BoxGroup("Tile Settings", order: 2), ShowIf("isDungeonScene")]
    public Color disabledTileColor;


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
        
        this.globalCtrl.enemyManager.enemySpawnPoints = this.enemySpawnPoints;

        this.globalState = this.globalCtrl.globalState;
        this.sceneState = this.globalState.sceneStates[this.id];

        if (!this.sceneState.isDungeonScene)
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

    [Button(ButtonSizes.Medium), BoxGroup("Enemy Spawn Points", order: 3), HideIf("isDungeonScene")]
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
