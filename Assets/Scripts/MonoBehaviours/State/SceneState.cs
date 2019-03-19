using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;
using UnityEngine;


[CreateAssetMenu]
public class SceneState : SerializedScriptableObject
{
    public bool isDungeonScene = false;

    [DictionaryDrawerSettings(KeyLabel = "LightSource ID", ValueLabel = "IsEnabled"), ShowIf("isDungeonScene")]
    public Point currentPositionPoint;

    [DictionaryDrawerSettings(KeyLabel = "LightSource ID", ValueLabel = "IsEnabled"), HideIf("isDungeonScene")]
    // TODO: Update during scene switch and save
    public Vector3 currentPositionVector;

    [DictionaryDrawerSettings(KeyLabel = "LightSource ID", ValueLabel = "IsEnabled"), ShowIf("isDungeonScene")]
    public Dictionary<string, bool> lightSources = new Dictionary<string, bool>();

    [HideIf("isDungeonScene")]
    public WalkableAreaMask walkableAreas = WalkableAreaMask.Walkable;

    [DictionaryDrawerSettings(IsReadOnly = true)]
    public Dictionary<string, InteractableState> interactables = new Dictionary<string, InteractableState>();

    [ShowInInspector, ShowIf("isDungeonScene"), BoxGroup("Map Stats"), LabelWidth(60), HorizontalGroup("Map Stats/General", 360)]
    public Point size {
        get {
            return (
                this.defaultMap.Any() ? this.defaultMap.Select(ts => (ts != null) ? ts.point : Point.zero).Max() + new Point(1, 1) : Point.zero
            );
        }
    }
    [ShowInInspector, ShowIf("isDungeonScene"), BoxGroup("Map Stats"), LabelWidth(60), HorizontalGroup("Map Stats/General")]
    public int total { get { return this.defaultMap.Count; } }

    [ShowInInspector, ShowIf("isDungeonScene"), BoxGroup("Map Stats"), LabelWidth(60), HorizontalGroup("Map Stats/States")]
    public int _blocked { get { return this.defaultMap.Count(ts => (ts != null) && ts.isBlocked); } }
    [ShowInInspector, ShowIf("isDungeonScene"), BoxGroup("Map Stats"), LabelWidth(60), HorizontalGroup("Map Stats/States")]
    public int _visible { get { return this.defaultMap.Count(ts => (ts != null) && ts.isVisible); } }
    [ShowInInspector, ShowIf("isDungeonScene"), BoxGroup("Map Stats"), LabelWidth(60), HorizontalGroup("Map Stats/States")]
    public int _default { get { return this.defaultMap.Count(ts => (ts != null) && !ts.isBlocked && !ts.isVisible); } }



    [HideInInspector]
    public List<TileSimple> defaultMap = new List<TileSimple>();


    public SceneState() { }


    public Dictionary<Point, TileSimple> GetCurrentMap()
    {
        return this.defaultMap.ToDictionary(ts => ts.point, ts => ts);
    }

    public Dictionary<Point, TileSimple> GetCurrentMap(HashSet<Point> playerLayer)
    {
        return this.defaultMap.ToDictionary(ts => ts.point, ts =>
            new TileSimple(ts.point, playerLayer.Contains(ts.point), ts.isBlocked)
        );
    }

    public void LoadFromSerializable(SceneStateSerializable serializedSceneState)
    {
        this.lightSources = serializedSceneState.lightSources;
        this.defaultMap = serializedSceneState.defaultMap;
    }
}


[Serializable]
public class SceneStateSerializable
{
    public Dictionary<string, bool> lightSources = new Dictionary<string, bool>();
    public List<TileSimple> defaultMap = new List<TileSimple>();



    public SceneStateSerializable(SceneState sceneState)
    {
        this.lightSources = sceneState.lightSources;
        this.defaultMap = sceneState.defaultMap;
    }
}


[Flags]
public enum WalkableAreaMask
{
    Walkable = 1 << 0,
    NotWalkable = 1 << 1,
    Jump = 1 << 2,
    WalkablePhaseOne = 1 << 3,
    All = Walkable | NotWalkable | Jump | WalkablePhaseOne
}
