using System.Collections;


public class GameOverReaction : IEndOfTurnReaction
{
    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
    }

    public IEnumerator React()
    {
        this.globalCtrl.playerActionManager.arePlayerControlsLocked = true;
        this.globalCtrl.gameOverFade.SetActive(true);

        yield return null;
    }
}
