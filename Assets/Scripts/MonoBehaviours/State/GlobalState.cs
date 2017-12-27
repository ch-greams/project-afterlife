using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu]
public class GlobalState : SerializedScriptableObject
{
    public Point currentPosition;
    public SceneType currentScene;
    public ObjectiveId currentObjective;

    public List<Item> inventory = new List<Item>();

    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
    public Dictionary<ObjectiveId, Objective> objectives = new Dictionary<ObjectiveId, Objective>();


    public void LoadFromSerializable(GlobalStateSerializable serializedGlobalState)
    {
        this.currentPosition = serializedGlobalState.currentPosition;
        this.currentScene = serializedGlobalState.currentScene;
        this.currentObjective = serializedGlobalState.currentObjective;

        foreach (KeyValuePair<ObjectiveId, Objective> kvp in this.objectives)
        {
            kvp.Value.LoadFromSerializable(serializedGlobalState.objectives[kvp.Key]);
        }
    }
}

[Serializable]
public class GlobalStateSerializable
{
    public Point currentPosition;
    public SceneType currentScene;
    public ObjectiveId currentObjective;
    public Dictionary<ObjectiveId, ObjectiveSerializable> objectives = new Dictionary<ObjectiveId, ObjectiveSerializable>();


    public GlobalStateSerializable(GlobalState globalState)
    {
        this.currentPosition = globalState.currentPosition;
        this.currentScene = globalState.currentScene;
        this.currentObjective = globalState.currentObjective;
        this.objectives = globalState.objectives
            .ToDictionary(kvp => kvp.Key, kvp => new ObjectiveSerializable(kvp.Value));
    }
}
