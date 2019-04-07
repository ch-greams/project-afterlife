using System.Collections;


public class PlayerWalkReaction : ITurnActionReaction
{
    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
    }

    public IEnumerator React()
    {
        Player player = this.globalCtrl.sceneCtrl.player;
        PlayerActionManager playerActionManager = this.globalCtrl.playerActionManager;
    
        if (playerActionManager.selectedTile)
        {
            yield return player.MoveToTile(playerActionManager.selectedTile);

            player.UpdateHighlightOnVisible(true);
        }

        playerActionManager.SelectActionType(PlayerActionType.Undefined);
    }
}
