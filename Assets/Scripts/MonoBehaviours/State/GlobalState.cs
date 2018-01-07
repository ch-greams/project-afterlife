using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu]
public class GlobalState : SerializedScriptableObject
{
    public Point currentPosition;
    public float currentVisibility;
    public SceneType currentScene;
    public ObjectiveId currentObjective;

    public List<Item> inventory = new List<Item>();

    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
    public Dictionary<ObjectiveId, Objective> objectives = new Dictionary<ObjectiveId, Objective>();

    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
    public Dictionary<SceneType, SceneState> sceneStates = new Dictionary<SceneType, SceneState>();


    public void LoadFromSerializable(GlobalStateSerializable serializedGlobalState)
    {
        this.currentPosition = serializedGlobalState.currentPosition;
        this.currentVisibility = serializedGlobalState.currentVisibility;
        this.currentScene = serializedGlobalState.currentScene;
        this.currentObjective = serializedGlobalState.currentObjective;

        foreach (KeyValuePair<ObjectiveId, Objective> kvp in this.objectives)
        {
            kvp.Value.LoadFromSerializable(serializedGlobalState.objectives[kvp.Key]);
        }

        foreach (KeyValuePair<SceneType, SceneState> kvp in this.sceneStates)
        {
            kvp.Value.LoadFromSerializable(serializedGlobalState.sceneStates[kvp.Key]);
        }
    }
}

[Serializable]
public class GlobalStateSerializable
{
    public Point currentPosition;
    public float currentVisibility;
    public SceneType currentScene;
    public ObjectiveId currentObjective;
    public Dictionary<ObjectiveId, ObjectiveSerializable> objectives = new Dictionary<ObjectiveId, ObjectiveSerializable>();
    public Dictionary<SceneType, SceneStateSerializable> sceneStates = new Dictionary<SceneType, SceneStateSerializable>();


    public GlobalStateSerializable(GlobalState globalState)
    {
        this.currentPosition = globalState.currentPosition;
        this.currentVisibility = globalState.currentVisibility;
        this.currentScene = globalState.currentScene;
        this.currentObjective = globalState.currentObjective;
        this.objectives = globalState.objectives
            .ToDictionary(kvp => kvp.Key, kvp => new ObjectiveSerializable(kvp.Value));
        this.sceneStates = globalState.sceneStates
            .ToDictionary(kvp => kvp.Key, kvp => new SceneStateSerializable(kvp.Value));
    }
}
