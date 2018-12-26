using System.Collections;
using Sirenix.OdinInspector;


public class InteractableDataReaction : IInteractableReaction
{
    public InteractableDataReactionType type;

    public bool useCurrentInteractable = false;
    [HideIf("useCurrentInteractable")]
    public Interactable interactable;


    public void Init(Interactable interactable)
    {
        if (this.useCurrentInteractable)
        {
            this.interactable = interactable;
        }
    }

    public IEnumerator React()
    {
        switch (this.type)
        {
            case InteractableDataReactionType.EnableInteractable:
                this.interactable.data.ToggleInteractable(true);
                break;
            case InteractableDataReactionType.DisableInteractable:
                this.interactable.data.ToggleInteractable(false);
                break;
            default:
                yield return null;
                break;
        }
    }
}

public enum InteractableDataReactionType
{
    EnableInteractable,
    DisableInteractable,
}