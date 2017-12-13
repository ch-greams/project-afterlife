using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ContainerReaction : IReaction
{
    public ContainerReactionType type;

    public ContainerType container;

    private SceneController sceneCtrl;


    public void Init(Interactable interactable)
    {
        this.sceneCtrl = interactable.sceneCtrl;
    }
    public IEnumerator React()
    {
        Inventory inventory = this.sceneCtrl.globalCtrl.inventory;
        Dictionary<ContainerType, List<Item>> containers = this.sceneCtrl.sceneState.containers;

        switch (this.type)
        {
            case ContainerReactionType.ADD_ITEMS_TO_INVENTORY:
                inventory.AddItems(containers[this.container]);
                containers[this.container] = new List<Item>();
                break;
            default:
                yield return null;
                break;
        }
    }
}

public enum ContainerReactionType
{
    ADD_ITEMS_TO_INVENTORY,
}