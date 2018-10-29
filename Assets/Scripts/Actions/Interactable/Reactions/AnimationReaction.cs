using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class AnimationReaction : IInteractableReaction
{
    public AnimationReactionType type = AnimationReactionType.DEFAULT_ANIMATOR;
    [ShowIf("type", AnimationReactionType.CUSTOM_ANIMATOR)]
    public Animator customAnimator;

    private List<string> animations
    {
        get
        {
            switch (this.type)
            {
                case AnimationReactionType.DEFAULT_ANIMATOR:
                    return new List<string>()
                    {
                        "OpenDoor",
                        "OpenElevator",
                    };
                case AnimationReactionType.DEFAULT_PLAYER_ANIMATOR:
                    return new List<string>()
                    {
                        "AttemptTake",
                        "HighTake",
                        "MedTake",
                        "LowTake",
                    };
                case AnimationReactionType.CUSTOM_ANIMATOR:
                default:
                    return new List<string>();
            }
        }
    }

    [ValueDropdown("animations")]
    public string animationName;
    public float timeoutAfterInSeconds;

    private Animator defaultAnimator;
    private int animationId;
    private WaitForSeconds timeoutAfter;


    public void Init(Interactable interactable)
    {
        this.animationId = Animator.StringToHash(this.animationName);
        this.timeoutAfter = new WaitForSeconds(this.timeoutAfterInSeconds);

        this.defaultAnimator =
            this.type == AnimationReactionType.DEFAULT_PLAYER_ANIMATOR
                ? interactable.sceneCtrl.player.characterAnimator
                : interactable.data.animator;

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
