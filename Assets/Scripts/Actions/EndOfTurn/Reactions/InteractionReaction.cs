using System.Collections;


public class InteractionReaction : IEndOfTurnReaction
{
    public InteractionReactionType type = InteractionReactionType.TriggerInteractable;

    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
    }

    public IEnumerator React()
    {
        switch (this.type)
        {
            case InteractionReactionType.TriggerInteractable:
                yield return this.TryTriggerInteractable();
                break;
            case InteractionReactionType.TrySelectInteractable:
                yield return this.TrySelectInteractable();
                break;
            default:
                break;
        }
    }

    private IEnumerator TryTriggerInteractable()
    {
        if (this.globalCtrl.playerActionManager.currentInteractable != null)
        {
            yield return this.globalCtrl.playerActionManager.currentInteractable.OnClickAsync();
        }
    }

    private IEnumerator TrySelectInteractable()
    {
        SceneController sceneCtrl = this.globalCtrl.sceneCtrl;
        Interactable interactable = sceneCtrl.GetInteractableInReach();

        if (interactable != null)
        {
            this.globalCtrl.playerActionManager.SelectInteractable(
                interactable: interactable,
                isDungeonScene: sceneCtrl.isDungeonScene
            );

            if (interactable.data.isAutoTriggerInteractable)
            {
                yield return this.TryTriggerInteractable();
            }
        }
        else
        {
            this.globalCtrl.playerActionManager.DeselectInteractable(
                isDungeonScene: sceneCtrl.isDungeonScene
            );
        }
    }
}


public enum InteractionReactionType
{
    TriggerInteractable,
    TrySelectInteractable,
}
