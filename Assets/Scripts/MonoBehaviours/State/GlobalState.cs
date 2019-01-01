using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu]
public class GlobalState : SerializedScriptableObject
{
    public Point currentPosition;
    public float currentVisibility;
    [ValueDropdown("sceneNames")]
    public string currentScene;
    public ObjectiveId currentObjective;

    public List<Item> inventory = new List<Item>();

    [DictionaryDrawerSettings, BoxGroup("Objectives")]
    public Dictionary<ObjectiveId, Objective> objectives = new Dictionary<ObjectiveId, Objective>();

    [DictionaryDrawerSettings]
    public Dictionary<string, SceneState> sceneStates = new Dictionary<string, SceneState>();


    public Dictionary<string, object> eotState = new Dictionary<string, object>();


    private List<string> sceneNames { get { return GlobalController.sceneNames; } }


    public T GetVariableFromState<T>(string key)
    {
        return (
            this.eotState != null && this.eotState.ContainsKey(key)
                ? (T)this.eotState[key]
                : default(T)
        );
    }

    public void SetVariableInState(string key, object value)
    {
        if (this.eotState != null)
        {
            this.eotState[key] = value;
        }
        else
        {
            this.eotState = new Dictionary<string, object>(){ { key, value } };
        }
    }

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

        foreach (KeyValuePair<string, SceneState> kvp in this.sceneStates)
        {
            kvp.Value.LoadFromSerializable(serializedGlobalState.sceneStates[kvp.Key]);
        }
    }


    [Button(ButtonSizes.Medium), BoxGroup("Objectives")]
    private void CollectObjectives()
    {
        this.objectives = new Dictionary<ObjectiveId, Objective>();

        string[] assets = AssetDatabase.FindAssets("t:Objective");

        foreach (string guid in assets)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Objective objective = AssetDatabase.LoadAssetAtPath<Objective>(assetPath);

            this.objectives.Add(objective.id, objective);
        }
    }
}

[Serializable]
public class GlobalStateSerializable
{
    public Point currentPosition;
    public float currentVisibility;
    public string currentScene;
    public ObjectiveId currentObjective;
    public Dictionary<ObjectiveId, ObjectiveSerializable> objectives = new Dictionary<ObjectiveId, ObjectiveSerializable>();
    public Dictionary<string, SceneStateSerializable> sceneStates = new Dictionary<string, SceneStateSerializable>();


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
