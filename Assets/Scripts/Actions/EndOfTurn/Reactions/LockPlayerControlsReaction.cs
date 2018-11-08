using System.Collections;


public class LockPlayerControlsReaction : IEndOfTurnReaction
{
    public LockPlayerControlsReactionType type = LockPlayerControlsReactionType.Undefined;

    private PlayerActionManager playerActionManager;


    public void Init(GlobalController globalCtrl)
    {
        this.playerActionManager = globalCtrl.playerActionManager;
    }

    public IEnumerator React()
    {
        switch (this.type)
        {
            case LockPlayerControlsReactionType.LockPlayerControls:
                this.playerActionManager.arePlayerControlsLocked = true;
                break;
            case LockPlayerControlsReactionType.LockPlayerControlsWithFade:
                this.playerActionManager.arePlayerControlsLocked = true;
                this.playerActionManager.enemyTurnFadeImage.SetActive(true);
                break;
            case LockPlayerControlsReactionType.UnlockPlayerControlsWithFade:
                this.playerActionManager.arePlayerControlsLocked = false;
                this.playerActionManager.enemyTurnFadeImage.SetActive(false);
                break;
            case LockPlayerControlsReactionType.Undefined:
            default:
                yield return null;
                break;
        }
    }
}

public enum LockPlayerControlsReactionType
{
    Undefined,
    LockPlayerControlsWithFade,
    UnlockPlayerControlsWithFade,
    LockPlayerControls,
}
