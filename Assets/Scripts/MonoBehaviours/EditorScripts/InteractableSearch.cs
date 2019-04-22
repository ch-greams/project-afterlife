using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Linq;
using System;


public class InteractableSearch : SerializedMonoBehaviour
{
    [BoxGroup("Condition Search"), NonSerialized, ShowInInspector]
    public IInteractableCondition conditionToFind;
    [BoxGroup("Reaction Search"), NonSerialized, ShowInInspector]
    public IInteractableReaction reactionToFind;

    [BoxGroup("Interactable Search"), ValueDropdown("interactableNames"), NonSerialized, ShowInInspector]
    public string interactablePathToFind;


    [ShowInInspector]
    private List<string> sceneNames
    {
        get { return GlobalController.sceneNames; }
    }


    private IEnumerable<string> interactableNames
    {
        get
        {
            foreach (SceneState sceneState in this.globalCtrl.globalState.sceneStates.Values)
            {
                foreach (string interactableId in sceneState.interactables.Keys)
                {
                    yield return string.Format("{0}/{1}", sceneState.name, interactableId);
                }
            }
        }
    }

    private GlobalController globalCtrl { get { return GameObject.FindObjectOfType<GlobalController>(); } }
    


    [BoxGroup("Condition Search"), Button(ButtonSizes.Medium), EnableIf("conditionToFind")]
    public void FindConditionReferences()
    {
        Debug.Log("TODO: " + this.conditionToFind.GetType().ToString());
    }

    private IEnumerable<string> FindReactionReferencesByType(Type reactionType)
    {
        foreach (string sceneName in this.sceneNames)
        {
            Scene scene = EditorSceneManager.OpenScene(
                scenePath: string.Format("Assets/Scenes/{0}.unity", sceneName),
                mode: OpenSceneMode.Additive
            );

            SceneController sceneCtrl = GameObject.FindObjectOfType<SceneController>();

            foreach (KeyValuePair<string, Interactable> interactableKvp in sceneCtrl.interactables)
            {
                foreach (InteractableAction action in interactableKvp.Value.clickActions)
                {
                    foreach (IInteractableReaction reaction in action.reactions)
                    {
                        if (reaction.GetType() == reactionType)
                        {
                            yield return string.Format(
                                format: "scene: {0} / interactable: {1} / action: {2}",
                                arg0: sceneCtrl.id,
                                arg1: interactableKvp.Key,
                                arg2: action.name
                            );
                        }
                    }
                }   
            }

            EditorSceneManager.CloseScene(scene, true);
        }
    }

    private IEnumerable<string> FindInteractableReferences(string sceneName, string interactableName)
    {
        foreach (string loadedSceneName in this.sceneNames)
        {
            Scene loadedScene = EditorSceneManager.OpenScene(
                scenePath: string.Format("Assets/Scenes/{0}.unity", loadedSceneName),
                mode: OpenSceneMode.Additive
            );

            SceneController sceneCtrl = GameObject.FindObjectOfType<SceneController>();

            foreach (KeyValuePair<string, Interactable> interactableKvp in sceneCtrl.interactables)
            {
                foreach (InteractableAction action in interactableKvp.Value.clickActions)
                {
                    foreach (InteractableDataReaction reaction in action.reactions.OfType<InteractableDataReaction>())
                    {
                        switch (reaction.item)
                        {
                            case InteractableDataReactionItem.InteractableInCurrentScene:
                                if (loadedSceneName == sceneName && reaction.interactable.name == interactableName)
                                {
                                    yield return string.Format(
                                        format: "scene: {0} / interactable: {1} / action: {2} / type: {3}",
                                        args: new string[] {
                                            sceneCtrl.id,
                                            interactableKvp.Key,
                                            action.name,
                                            reaction.type.ToString()
                                        }
                                    );
                                }
                                break;
                            case InteractableDataReactionItem.InteractableInOtherScene:
                                if (reaction.sceneState.name == sceneName && reaction.interactableName == interactableName)
                                {
                                    yield return string.Format(
                                        format: "scene: {0} / interactable: {1} / action: {2} / type: {3}",
                                        args: new string[] {
                                            sceneCtrl.id,
                                            interactableKvp.Key,
                                            action.name,
                                            reaction.type.ToString()
                                        }
                                    );
                                }
                                break;
                            case InteractableDataReactionItem.CurrentInteractable:
                            default:
                                break;
                        }
                    }
                }   
            }

            EditorSceneManager.CloseScene(loadedScene, true);
        }
    }


    [BoxGroup("Reaction Search"), Button(ButtonSizes.Medium), EnableIf("reactionToFind")]
    public void FindReactionReferences()
    {
        Type reactionType = this.reactionToFind.GetType();
        IEnumerable<string> reactionPaths = this.FindReactionReferencesByType(reactionType);

        foreach (string item in reactionPaths)
        {
            Debug.Log(item);
        }
    }



    
    [BoxGroup("Interactable Search"), Button(ButtonSizes.Medium), DisableIf("interactablePathToFind", null)]
    public void FindInteractableReferences()
    {
        string[] interactablePathArr = this.interactablePathToFind.Split('/');

        string sceneName = interactablePathArr[0];
        string interactableName = interactablePathArr[1];

        IEnumerable<string> interactablePaths = this.FindInteractableReferences(
            sceneName: sceneName,
            interactableName: interactableName
        );

        foreach (string item in interactablePaths)
        {
            Debug.Log(item);
        }
    }
}
