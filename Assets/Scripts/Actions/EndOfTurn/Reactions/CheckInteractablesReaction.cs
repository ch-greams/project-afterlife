using System.Collections;


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
            .Find(interactable =>
                interactable.data.isInteractableActive && interactable.data.reachablePoints.Contains(sceneCtrl.player.tile.point)
            );

        this.globalCtrl.playerActionManager.TrySelectInteractable(currentInteractable);

        yield return null;
    }
}

