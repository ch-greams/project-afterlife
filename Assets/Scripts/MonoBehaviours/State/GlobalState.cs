using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu]
public class GlobalState : SerializedScriptableObject
{
    public SceneType currentScene;

    public List<Item> playerInventory = new List<Item>();

    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
    public Dictionary<SceneType, SceneState> sceneStates = new Dictionary<SceneType, SceneState>();

    public void Awake()
    {
        this.sceneStates = this.sceneStates.ToDictionary(kvp => kvp.Key, kvp => ScriptableObject.Instantiate(kvp.Value));
    }
}
