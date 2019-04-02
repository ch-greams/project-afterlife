using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class InteractableDataReaction : IInteractableReaction
{
    public InteractableDataReactionType type;
    public InteractableDataReactionItem item;

    
    [ShowIf("item", InteractableDataReactionItem.InteractableInCurrentScene)]
    public Interactable interactable;

    // NOTE: Use only as a reference for constant values in there
    [ShowIf("item", InteractableDataReactionItem.InteractableInOtherScene), Required]
    public SceneState sceneState;

    [ShowIf("item", InteractableDataReactionItem.InteractableInOtherScene), ValueDropdown("interactableNames")]
    public string interactableName;

    private List<string> interactableNames
    {
        get
        {
            return (this.sceneState != null)
                ? new List<string>(this.sceneState.interactables.Keys)
                : new List<string>();
        }
    }
    
    private GlobalController globalCtrl;



    public void Init(Interactable interactable)
    {
        this.globalCtrl = interactable.sceneCtrl.globalCtrl;

        if (this.item == InteractableDataReactionItem.CurrentInteractable)
        {
            this.interactable = interactable;
        }
    }

    public IEnumerator React()
    {
        switch (this.type)
        {
            case InteractableDataReactionType.EnableAndShowInteractable:
                this.ToggleBasedOnItem(true, true);
                break;
            case InteractableDataReactionType.DisableAndHideInteractable:
                this.ToggleBasedOnItem(false, false);
                break;
            case InteractableDataReactionType.EnableInteractable:
                this.ToggleBasedOnItem(true);
                break;
            case InteractableDataReactionType.DisableInteractable:
                this.ToggleBasedOnItem(false);
                break;
            default:
                yield return null;
                break;
        }
    }

    private void ToggleBasedOnItem(bool enabled)
    {
        if (this.item == InteractableDataReactionItem.InteractableInOtherScene)
        {
            SceneState sceneState = this.globalCtrl.globalState.sceneStates[this.sceneState.name];
            InteractableState interactableState = sceneState.interactables[this.interactableName];

            interactableState.enabled = enabled;
        }
        else
        {
            this.interactable.IsEnabled = enabled;
            if (!enabled)
            {
                this.TryDeselectInteractable();
            }
        }
    }

    private void ToggleBasedOnItem(bool enabled, bool visible)
    {
        if (this.item == InteractableDataReactionItem.InteractableInOtherScene)
        {
            SceneState sceneState = this.globalCtrl.globalState.sceneStates[this.sceneState.name];
            InteractableState interactableState = sceneState.interactables[this.interactableName];

            interactableState.Update(enabled, visible);
        }
        else
        {
            this.interactable.ToggleInteractable(enabled, visible);
            if (!enabled)
            {
                this.TryDeselectInteractable();
            }
        }
    }

    /// <summary>
    /// Deselect interactable if currently selected in PlayerActionManager
    /// </summary>
    private void TryDeselectInteractable()
    {
        PlayerActionManager playerActionManager = this.globalCtrl.playerActionManager;

        if (playerActionManager.currentInteractable == this.interactable)
        {
            playerActionManager.DeselectInteractable(this.globalCtrl.sceneCtrl.isDungeonScene);
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

public enum InteractableDataReactionItem
{
    CurrentInteractable,
    InteractableInCurrentScene,
    InteractableInOtherScene,
}
