﻿using System.Linq;
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
        this.globalState = this.globalCtrl.globalState;
        this.sceneState = this.globalState.sceneStates[this.id];

        this.player.UpdatePlayer(this.tiles.Find(t => t.point == this.globalState.currentPosition), this.globalState.currentVisibility);

        this.UpdateTiles(this.player.tile, this.sceneState.visibleByDefault);
    }

    // NOTE: ~100Mb (60%/40% in first 2 steps) of garbage from full 20 visibility
    public void UpdateTiles(Tile playerTile, bool visibleByDefault)
    {   
        // TODO: Rework this
        // - No need for (whole map)/(HighlightedTiles) update if visibleByDefault
        // - ...

        List<Point> playerTiles = playerTile.GetTiles(this.player.visibleRange, false);

        Dictionary<Point, TileSimple> currentMap = (
            visibleByDefault
                ? this.sceneState.GetCurrentMap()
                : this.sceneState.GetCurrentMap(playerTiles)
        );

        foreach (Tile t in this.tiles)
        {
            TileData tileData = t.obj.GetComponent<Interactable>().data as TileData;
            TileSimple tileSimple = currentMap[t.point];
            tileData.RefreshTileState(tileSimple.isVisible, tileSimple.isBlocked, false);
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
            // TileData tileData = tile.obj.GetComponent<Interactable>().data as TileData;
            // TileState defaultTileState = this.sceneState.defaultMap.Find(ts => ts.point == tile.point).state;
            // TileState tileState = (defaultTileState == TileState.Hidden) ? TileState.Active : defaultTileState;
            // tileData.RefreshTileState(tileState, false);

            TileData tileData = tile.obj.GetComponent<Interactable>().data as TileData;
            tileData.RefreshTileState(true, tile.isBlocked, false);
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
