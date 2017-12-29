using System.Collections;
using Sirenix.OdinInspector;


public class SceneReaction : IReaction
{
    public SceneReactionType type;
    [ShowIf("type", SceneReactionType.DEFAULT)]
    public SceneType scene;
    [ShowIf("type", SceneReactionType.DEFAULT)]
    public Point startPoint;

    private GlobalController globalCtrl;
    private SceneType defaultScene;
    private Point defaultPosition;


    public void Init(Interactable interactable)
    {
        this.globalCtrl = interactable.sceneCtrl.globalCtrl;

        switch (interactable.GetType().Name)
        {
            case "DoorInteractable":
                DoorInteractable di = interactable as DoorInteractable;
                this.defaultScene = di.door.toScene;
                this.defaultPosition = di.door.exitPoint;
                break;
            default:
                break;
        }
    }
    public IEnumerator React()
    {
        switch (this.type)
        {
            case SceneReactionType.DOOR_TRIGGER:
                this.globalCtrl.UpdatePlayerPosition(this.defaultScene, this.defaultPosition);
                SceneManager.FadeAndLoadScene(this.defaultScene.ToString());
                break;
            case SceneReactionType.DEFAULT:
                this.globalCtrl.UpdatePlayerPosition(this.scene, this.startPoint);
                SceneManager.FadeAndLoadScene(this.scene.ToString());
                break;
            default:
                yield return null;
                break;
        }
    }
}

public enum SceneReactionType
{
    DEFAULT,
    DOOR_TRIGGER,
}
