using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;


public class AnimationReaction : IReaction
{
    public AnimationReactionType type = AnimationReactionType.DEFAULT_ANIMATOR;
    [ShowIf("type", AnimationReactionType.CUSTOM_ANIMATOR)]
    public Animator customAnimator;
    public string animationName;
    public float timeoutAfterInSeconds;

    private Animator defaultAnimator;
    private int animationId;
    private WaitForSeconds timeoutAfter;


    public void Init(Interactable interactable)
    {
        this.animationId = Animator.StringToHash(this.animationName);
        this.timeoutAfter = new WaitForSeconds(this.timeoutAfterInSeconds);

        switch (interactable.GetType().Name)
        {
            case "ContainerInteractable":
                ContainerInteractable ci = interactable as ContainerInteractable;
                this.defaultAnimator =
                    this.type == AnimationReactionType.DEFAULT_PLAYER_ANIMATOR
                        ? ci.sceneCtrl.playerCtrl.playerAnimator
                        : ci.container.animator;
                break;
            case "DoorInteractable":
                DoorInteractable di = interactable as DoorInteractable;
                this.defaultAnimator =
                    this.type == AnimationReactionType.DEFAULT_PLAYER_ANIMATOR
                        ? di.sceneCtrl.playerCtrl.playerAnimator
                        : di.door.animator;
                break;
            default:
                break;
        }

        if (this.defaultAnimator == null)
        {
            Debug.LogWarningFormat("defaultAnimator for {0} isn't set", interactable.GetType().Name);
        }
    }
    public IEnumerator React()
    {
        switch (this.type)
        {
            case AnimationReactionType.DEFAULT_ANIMATOR:
            case AnimationReactionType.DEFAULT_PLAYER_ANIMATOR:
                this.defaultAnimator.SetTrigger(this.animationId);
                break;
            case AnimationReactionType.CUSTOM_ANIMATOR:
                this.customAnimator.SetTrigger(this.animationId);
                break;
            default:
                break;
        }

        if (this.timeoutAfterInSeconds > 0)
        {
            yield return this.timeoutAfter;
        }
    }
}

public enum AnimationReactionType
{
    DEFAULT_ANIMATOR,
    DEFAULT_PLAYER_ANIMATOR,
    CUSTOM_ANIMATOR,
}
