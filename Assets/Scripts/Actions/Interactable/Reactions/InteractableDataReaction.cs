using System.Collections;
using Sirenix.OdinInspector;


public class InteractableDataReaction : IInteractableReaction
{
    public InteractableDataReactionType type;

    public bool useCurrentInteractable = false;
    [HideIf("useCurrentInteractable")]
    public Interactable interactable;

    private SceneController sceneCtrl;


    public void Init(Interactable interactable)
    {
        this.sceneCtrl = interactable.sceneCtrl;

        if (this.useCurrentInteractable)
        {
            this.interactable = interactable;
        }
    }

    public IEnumerator React()
    {
        PlayerActionManager playerActionManager = this.sceneCtrl.globalCtrl.playerActionManager;

        switch (this.type)
        {
            case InteractableDataReactionType.EnableAndShowInteractable:
                this.interactable.ToggleInteractable(true, true);
                break;
            case InteractableDataReactionType.DisableAndHideInteractable:
                this.interactable.ToggleInteractable(false, false);
                this.TryDeselectInteractable();
                break;
            case InteractableDataReactionType.EnableInteractable:
                this.interactable.IsEnabled = true;
                break;
            case InteractableDataReactionType.DisableInteractable:
                this.interactable.IsEnabled = false;
                this.TryDeselectInteractable();
                break;
            default:
                yield return null;
                break;
        }
    }

    /// <summary>
    /// Deselect interactable if currently selected in PlayerActionManager
    /// </summary>
    private void TryDeselectInteractable()
    {
        PlayerActionManager playerActionManager = this.sceneCtrl.globalCtrl.playerActionManager;

        if (playerActionManager.currentInteractable == this.interactable)
        {
            playerActionManager.TrySelectInteractable(null, this.sceneCtrl.sceneState.isDungeonScene);
        }
    }
}

public enum InteractableDataReactionType
{
    EnableAndShowInteractable,
    DisableAndHideInteractable,
    EnableInteractable,
    DisableInteractable,
}
