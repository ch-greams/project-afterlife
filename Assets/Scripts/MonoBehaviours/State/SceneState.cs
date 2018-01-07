using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;
using UnityEngine;


[CreateAssetMenu]
public class SceneState : SerializedScriptableObject
{
    public Dictionary<string, bool> lightSources = new Dictionary<string, bool>();

    [DictionaryDrawerSettings(IsReadOnly = true)]
    public Dictionary<Point, TileState> defaultMap = new Dictionary<Point, TileState>();


    public SceneState() { }

    public Dictionary<Point, TileState> GetCurrentMap(List<Point> playerLayer)
    {
        return this.defaultMap.ToDictionary(kvp => kvp.Key, kvp => 
            (kvp.Value == TileState.Hidden)
                ? (playerLayer.Contains(kvp.Key) ? TileState.Active : TileState.Hidden)
                : TileState.Disabled
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
    public Dictionary<Point, TileState> defaultMap = new Dictionary<Point, TileState>();


    public SceneStateSerializable(SceneState sceneState)
    {
        this.lightSources = sceneState.lightSources;
        this.defaultMap = sceneState.defaultMap;
    }
}
