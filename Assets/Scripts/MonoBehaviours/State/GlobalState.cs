using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu]
public class GlobalState : SerializedScriptableObject
{
    public float currentVisibility;

    [ValueDropdown("sceneNames")]
    public string currentScene;

    [ValueDropdown("objectiveNames")]
    public string currentObjective;

    public List<Item> inventory = new List<Item>();

    [DictionaryDrawerSettings, BoxGroup("Objectives")]
    public Dictionary<string, Objective> objectives = new Dictionary<string, Objective>();

    [DictionaryDrawerSettings]
    public Dictionary<string, SceneState> sceneStates = new Dictionary<string, SceneState>();


    [BoxGroup("Variables by Type")]
    public Dictionary<string, bool> booleanParameters = new Dictionary<string, bool>();

    [BoxGroup("Variables by Type")]
    public Dictionary<string, int> integerParameters = new Dictionary<string, int>();


    private List<string> sceneNames { get { return GlobalController.sceneNames; } }
    private List<string> objectiveNames { get { return this.objectives.Keys.ToList(); } }



    public bool GetBooleanParameterFromState(string key)
    {
        return (
            this.booleanParameters != null && this.booleanParameters.ContainsKey(key)
                ? this.booleanParameters[key]
                : false
        );
    }

    public int GetIntegerParameterFromState(string key)
    {
        return (
            this.integerParameters != null && this.integerParameters.ContainsKey(key)
                ? this.integerParameters[key]
                : 0
        );
    }


    public void SetBooleanParameterInState(string key, bool value)
    {
        if (this.booleanParameters != null)
        {
            this.booleanParameters[key] = value;
        }
        else
        {
            this.booleanParameters = new Dictionary<string, bool>(){ { key, value } };
        }
    }

    public void SetIntegerParameterInState(string key, int value)
    {
        if (this.integerParameters != null)
        {
            this.integerParameters[key] = value;
        }
        else
        {
            this.integerParameters = new Dictionary<string, int>(){ { key, value } };
        }
    }


    public void LoadFromSerializable(GlobalStateSerializable serializedGlobalState)
    {
        this.currentVisibility = serializedGlobalState.currentVisibility;
        this.currentScene = serializedGlobalState.currentScene;
        this.currentObjective = serializedGlobalState.currentObjective;

        foreach (KeyValuePair<string, Objective> kvp in this.objectives)
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
        this.objectives = new Dictionary<string, Objective>();

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
    public float currentVisibility;
    public string currentScene;
    public string currentObjective;
    public Dictionary<string, ObjectiveSerializable> objectives = new Dictionary<string, ObjectiveSerializable>();
    public Dictionary<string, SceneStateSerializable> sceneStates = new Dictionary<string, SceneStateSerializable>();


    public GlobalStateSerializable(GlobalState globalState)
    {
        this.currentVisibility = globalState.currentVisibility;
        this.currentScene = globalState.currentScene;
        this.currentObjective = globalState.currentObjective;
        this.objectives = globalState.objectives
            .ToDictionary(kvp => kvp.Key, kvp => new ObjectiveSerializable(kvp.Value));
        this.sceneStates = globalState.sceneStates
            .ToDictionary(kvp => kvp.Key, kvp => new SceneStateSerializable(kvp.Value));
    }
}
