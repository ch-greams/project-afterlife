using System.Collections;


public interface IReaction
{
    void Init(Interactable interactable);
    IEnumerator React();
}
