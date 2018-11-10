using System.Collections;
using System.Collections.Generic;


public class PlayerTorchReaction : IEndOfTurnReaction
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
            player.characterTransform.LookAt(playerActionManager.selectedTile.obj.transform.position);

            player.KillEnemiesOnTiles(playerActionManager.selectedTiles);
        }
    
        playerActionManager.UpdateSelectedTiles(new HashSet<Tile>());

        playerActionManager.SelectActionType(PlayerActionType.Undefined);

        yield return null;
    }
}
