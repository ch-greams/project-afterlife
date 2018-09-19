using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;
using UnityEngine;


[CreateAssetMenu]
public class SceneState : SerializedScriptableObject
{
    public bool visibleByDefault = false;
    [HideIf("visibleByDefault")]
    [DictionaryDrawerSettings(KeyLabel = "LightSource ID", ValueLabel = "IsEnabled")]
    public Dictionary<string, bool> lightSources = new Dictionary<string, bool>();

    [ShowInInspector, BoxGroup("Map Stats"), LabelWidth(60), HorizontalGroup("Map Stats/General", 360)]
    public Point size {
        get {
            return (
                this.defaultMap.Any() ? this.defaultMap.Select(ts => ts.point).Max() + new Point(1, 1) : new Point()
            );
        }
    }
    [ShowInInspector, BoxGroup("Map Stats"), LabelWidth(60), HorizontalGroup("Map Stats/General")]
    public int total { get { return this.defaultMap.Count; } }

    [ShowInInspector, BoxGroup("Map Stats"), LabelWidth(60), HorizontalGroup("Map Stats/States")]
    public int _blocked { get { return this.defaultMap.Count(ts => ts.isBlocked); } }
    [ShowInInspector, BoxGroup("Map Stats"), LabelWidth(60), HorizontalGroup("Map Stats/States")]
    public int _visible { get { return this.defaultMap.Count(ts => ts.isVisible); } }
    [ShowInInspector, BoxGroup("Map Stats"), LabelWidth(60), HorizontalGroup("Map Stats/States")]
    public int _default { get { return this.defaultMap.Count(ts => !ts.isBlocked && !ts.isVisible); } }



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
