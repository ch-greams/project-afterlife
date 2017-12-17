using System.Collections.Generic;
using System.Linq;


public class ContainerCondition : ICondition
{
    public ContainerConditionType type = ContainerConditionType.IS_NOT_EMPTY;

    private ContainerType containerType;

    private SceneController sceneCtrl;


    public void Init(Interactable interactable)
    {
        this.sceneCtrl = interactable.sceneCtrl;

        switch (interactable.GetType().Name)
        {
            case "ContainerInteractable":
                this.containerType = (interactable as ContainerInteractable).container.type;
                break;
            default:
                break;
        }
    }

    public bool IsValid()
    {
        List<Item> items = this.sceneCtrl.sceneState.containers[this.containerType];

        switch (this.type)
        {
            case ContainerConditionType.IS_NOT_EMPTY:
                return items.Any();
            case ContainerConditionType.IS_EMPTY:
                return !items.Any();
            default:
                return false;
        }
    }
}

public enum ContainerConditionType
{
    IS_NOT_EMPTY,
    IS_EMPTY,
}
