﻿using System.Collections;
using UnityEngine;


public class CheckInteractablesReaction : IEndOfTurnReaction
{
    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
    }

    public IEnumerator React()
    {
        SceneController sceneCtrl = this.globalCtrl.sceneCtrl;

        Interactable currentInteractable = sceneCtrl.interactables
            .Find(interactable => interactable.data.reachablePoints.Contains(sceneCtrl.player.tile.point));

        if (currentInteractable)
        {
            Debug.Log("Interactable can be triggered");
        }

        this.globalCtrl.playerActionManager.TrySelectInteractable(currentInteractable);

        yield return null;
    }
}

