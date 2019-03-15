using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SceneReaction : IInteractableReaction
{
    [ValueDropdown("sceneNames")]
    public string sceneName;
    public bool isDungeonScene = false;

    [ShowIf("isDungeonScene")]
    public Point startPositionPoint;
    [HideIf("isDungeonScene")]
    public Vector3 startPositionVector;

    private GlobalController globalCtrl;
    private List<string> sceneNames { get { return GlobalController.sceneNames; } }


    public void Init(Interactable interactable)
    {
        this.globalCtrl = interactable.sceneCtrl.globalCtrl;
    }

    public IEnumerator React()
    {
        if (this.isDungeonScene)
        {
            this.globalCtrl.UpdatePlayerPosition(this.sceneName, this.startPositionPoint);
        }
        else
        {
            this.globalCtrl.UpdatePlayerPosition(this.sceneName, this.startPositionVector);
        }

        SceneManager.FadeAndLoadScene(this.sceneName);

        yield return null;
    }
}
