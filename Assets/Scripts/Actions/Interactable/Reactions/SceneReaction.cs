using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class SceneReaction : IInteractableReaction
{
    public SceneReactionType type;

    [ShowIf("type", SceneReactionType.DEFAULT), ValueDropdown("sceneNames")]
    public string sceneName;

    [ShowIf("type", SceneReactionType.DEFAULT)]
    public Point startPoint;

    private GlobalController globalCtrl;
    private string defaultScene;
    private Point defaultPosition;
    private List<string> sceneNames { get { return GlobalController.sceneNames; } }


    public void Init(Interactable interactable)
    {
        this.globalCtrl = interactable.sceneCtrl.globalCtrl;

        switch (interactable.data.GetType().Name)
        {
            case "DoorData":
                DoorData dd = interactable.data as DoorData;
                this.defaultScene = dd.sceneName;
                this.defaultPosition = dd.exitPosition;
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
                SceneManager.FadeAndLoadScene(this.defaultScene);
                break;
            case SceneReactionType.DEFAULT:
                this.globalCtrl.UpdatePlayerPosition(this.sceneName, this.startPoint);
                SceneManager.FadeAndLoadScene(this.sceneName);
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
