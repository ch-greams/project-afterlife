using System.Collections.Generic;
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

}
