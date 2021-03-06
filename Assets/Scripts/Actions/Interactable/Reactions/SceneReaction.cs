﻿using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;


public class SceneReaction : IInteractableReaction
{
    // NOTE: Use only as a reference for constant values in there
    [Required]
    public SceneState scene;

    [ShowIf("isDungeonScene")]
    public Point startPositionPoint;
    [HideIf("isDungeonScene")]
    public Vector3 startPositionVector;

    private GlobalController globalCtrl;
    private bool isDungeonScene { get { return this.scene ? this.scene.isDungeonScene : false; } }


    public void Init(Interactable interactable)
    {
        this.globalCtrl = interactable.sceneCtrl.globalCtrl;
    }

    public IEnumerator React()
    {
        // NOTE: Update state        
        if (this.scene.isDungeonScene)
        {
            this.globalCtrl.UpdatePlayerPosition(this.scene.name, this.startPositionPoint);
        }
        else
        {
            this.globalCtrl.UpdatePlayerPosition(this.scene.name, this.startPositionVector);
        }

        // NOTE: Skip actions in EoT
        if (this.globalCtrl.sceneCtrl.isDungeonScene)
        {
            this.globalCtrl.turnActionManager.TrySkipActions();
        }

        // NOTE: Load scene
        yield return this.globalCtrl.sceneManager.FadeAndLoadScene(this.scene.name);
    }
}
