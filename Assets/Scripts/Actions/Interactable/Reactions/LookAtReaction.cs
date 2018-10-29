using System.Collections;
using UnityEngine;


public class LookAtReaction : IInteractableReaction
{
    public LookAtReactionType type = LookAtReactionType.DEFAULT_ACTOR_AND_TARGET;

    private Transform defaultActor;
    private Transform defaultTarget;


    public void Init(Interactable interactable)
    {
        this.defaultActor = interactable.sceneCtrl.player.characterTransform;
        this.defaultTarget = interactable.data.gameObject.transform;
    }
    public IEnumerator React()
    {
        switch (this.type)
        {
            case LookAtReactionType.DEFAULT_ACTOR_AND_TARGET:
                this.defaultActor.LookAt(this.defaultTarget.position);
                float delta = -1 * this.defaultActor.rotation.eulerAngles.x;
                this.defaultActor.Rotate(delta, 0, 0);
                break;
            default:
                yield return null;
                break;
        }
    }
}

public enum LookAtReactionType
{
    DEFAULT_ACTOR_AND_TARGET,
}
