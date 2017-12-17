using System.Collections;
using Sirenix.OdinInspector;


public class DoorReaction : IReaction
{
    public DoorReactionType type = DoorReactionType.OPEN_DEFAULT_DOOR;
    [ShowIf("type", DoorReactionType.OPEN_CUSTOM_DOOR)]
    public SceneType customSceneType;
    [ShowIf("type", DoorReactionType.OPEN_CUSTOM_DOOR)]
    public DoorType customDoorType;
    private DoorType defaultDoorType;
    private SceneController sceneCtrl;


    public void Init(Interactable interactable)
    {
        this.sceneCtrl = interactable.sceneCtrl;

        switch (interactable.GetType().Name)
        {
            case "DoorInteractable":
                this.defaultDoorType = (interactable as DoorInteractable).door.type;
                break;
            default:
                break;
        }
    }
    public IEnumerator React()
    {
        switch (this.type)
        {
            case DoorReactionType.OPEN_DEFAULT_DOOR:
                this.sceneCtrl.sceneState.doors[this.defaultDoorType] = true;
                break;
            case DoorReactionType.OPEN_CUSTOM_DOOR:
                this.sceneCtrl.globalState.sceneStates[this.customSceneType].doors[this.customDoorType] = true;
                break;
            default:
                yield return null;
                break;
        }
    }
}

public enum DoorReactionType
{
    OPEN_DEFAULT_DOOR,
    OPEN_CUSTOM_DOOR,
}
