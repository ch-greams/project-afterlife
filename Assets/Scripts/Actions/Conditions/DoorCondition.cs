using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DoorCondition : ICondition
{
    public DoorConditionType type;

    private DoorType doorType;

    private SceneController sceneCtrl;


    public void Init(Interactable interactable)
    {
        this.sceneCtrl = interactable.sceneCtrl;

        switch (interactable.GetType().Name)
        {
            case "DoorInteractable":
                this.doorType = (interactable as DoorInteractable).door.type;
                break;
            default:
                break;
        }
    }
    public bool IsValid()
    {
        switch (this.type)
        {
            case DoorConditionType.IS_OPEN:
                return this.sceneCtrl.sceneState.doors[this.doorType];
            case DoorConditionType.IS_CLOSED:
                return !this.sceneCtrl.sceneState.doors[this.doorType];
            default:
                return false;
        }
    }
}

public enum DoorConditionType
{
    IS_OPEN,
    IS_CLOSED,
}