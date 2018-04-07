using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;
using UnityEngine;


[CreateAssetMenu]
public class SceneState : SerializedScriptableObject
{
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
    public int active { get { return this.defaultMap.Count(ts => ts.state == TileState.Active); } }
    [ShowInInspector, BoxGroup("Map Stats"), LabelWidth(60), HorizontalGroup("Map Stats/States")]
    public int hidden { get { return this.defaultMap.Count(ts => ts.state == TileState.Hidden); } }
    [ShowInInspector, BoxGroup("Map Stats"), LabelWidth(60), HorizontalGroup("Map Stats/States")]
    public int disabled { get { return this.defaultMap.Count(ts => ts.state == TileState.Disabled); } }


    [HideInInspector]
    public List<TileSimple> defaultMap = new List<TileSimple>();


    public SceneState() { }

    public List<TileSimple> GetCurrentMap(List<Point> playerLayer)
    {
        return this.defaultMap
            .Select(ts =>
                new TileSimple(
                    ts.point,
                    (ts.state == TileState.Hidden)
                        ? (playerLayer.Contains(ts.point) ? TileState.Active : TileState.Hidden)
                        : TileState.Disabled
                )
            )
            .ToList();
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
