using System.Collections;


public class LockPlayerControlsReaction : IEndOfTurnReaction
{
    public LockPlayerControlsReactionType type = LockPlayerControlsReactionType.Undefined;

    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
    }

    // TODO: Clean up this
    public IEnumerator React()
    {
        PlayerActionManager playerActionManager = globalCtrl.playerActionManager;
        PlayerActionInterface interfaceElements = playerActionManager.interfaceElements;
        
        switch (this.type)
        {
            case LockPlayerControlsReactionType.LockPlayerControls:
                playerActionManager.arePlayerControlsLocked = true;                 // NOTE: Disable player controls
                interfaceElements.interactionButton.gameObject.SetActive(false);    // NOTE: Show use button
                interfaceElements.skipTurnButton.gameObject.SetActive(false);       // NOTE: Hide skip button
                break;
            case LockPlayerControlsReactionType.ShowFade:
                playerActionManager.enemyTurnFadeImage.SetActive(true);             // NOTE: Show fade canvas
                break;
            case LockPlayerControlsReactionType.UnlockPlayerControlsWithFade:
                playerActionManager.arePlayerControlsLocked = false;                // NOTE: Enable player controls
                playerActionManager.enemyTurnFadeImage.SetActive(false);            // NOTE: Hide fade canvas
                interfaceElements.skipTurnButton.gameObject.SetActive(true);        // NOTE: Show skip button
                break;
            case LockPlayerControlsReactionType.GameOver:
                playerActionManager.arePlayerControlsLocked = true;                 // NOTE: Disable player controls
                playerActionManager.enemyTurnFadeImage.SetActive(false);            // NOTE: Hide fade canvas
                interfaceElements.interactionButton.gameObject.SetActive(false);    // NOTE: Show use button
                interfaceElements.skipTurnButton.gameObject.SetActive(false);       // NOTE: Hide skip button

                playerActionManager.gameOverFade.SetActive(true);                   // NOTE: Show GameOver fade canvas
                this.globalCtrl.endOfTurnActionManager.TrySkipActions();            // NOTE: Skip following actions
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
    ShowFade,
    UnlockPlayerControlsWithFade,
    LockPlayerControls,
    GameOver,
}
