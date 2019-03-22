using System.Collections;
using System.Linq;


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
            .FirstOrDefault(kvp =>
                kvp.Value.data.isInteractableActive && kvp.Value.data.reachablePoints.Contains(sceneCtrl.player.tile.point)
            )
            .Value;

        if (currentInteractable != null)
        {
            this.globalCtrl.playerActionManager.SelectInteractable(
                interactable: currentInteractable,
                isDungeonScene: sceneCtrl.sceneState.isDungeonScene,
                buttonLabel: currentInteractable.data.actionLabel
            );
        }
        else
        {
            this.globalCtrl.playerActionManager.DeselectInteractable(sceneCtrl.sceneState.isDungeonScene);
        }

        yield return null;
    }
}

