using System.Collections;
using UnityEngine;


public class LookAtReaction : IReaction
{
    public LookAtReactionType type = LookAtReactionType.DEFAULT_ACTOR_AND_TARGET;

    private Transform defaultActor;
    private Transform defaultTarget;


    public void Init(Interactable interactable)
    {
        switch (interactable.GetType().Name)
        {
            case "ContainerInteractable":
                ContainerInteractable ci = interactable as ContainerInteractable;
                this.defaultActor = ci.sceneCtrl.player.characterTransform;
                this.defaultTarget = ci.container.obj.transform;
                break;
            case "DoorInteractable":
                DoorInteractable di = interactable as DoorInteractable;
                this.defaultActor = di.sceneCtrl.player.characterTransform;
                this.defaultTarget = di.door.obj.transform;
                break;
            default:
                break;
        }
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
