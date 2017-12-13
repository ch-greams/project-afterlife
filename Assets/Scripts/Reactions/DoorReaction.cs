using System.Collections;


public class DoorReaction : IReaction
{
    public DoorReactionType type = DoorReactionType.OPEN_DOOR;
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
    public IEnumerator React()
    {
        switch (this.type)
        {
            case DoorReactionType.OPEN_DOOR:
                this.sceneCtrl.sceneState.doors[this.doorType] = true;
                break;
            default:
                yield return null;
                break;
        }
    }
}

public enum DoorReactionType
{
    OPEN_DOOR,
}
