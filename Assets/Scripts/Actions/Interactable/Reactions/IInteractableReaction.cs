using System.Collections;


public interface IInteractableReaction
{
    void Init(Interactable interactable);
    IEnumerator React();
}
