using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerFlashlightReaction : ITurnActionReaction
{
    [Range(0, 2)]
    public float animationTimeout = 0.25F;
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
            player.characterTransform.LookAt(playerActionManager.selectedTile.gameObject.transform.position);
            player.flashlightRay.SetActive(true);

            player.KillEnemiesOnTiles(playerActionManager.selectedTiles);

            yield return new WaitForSeconds(this.animationTimeout);

            player.flashlightRay.SetActive(false);
        }
    
        playerActionManager.UpdateSelectedTiles(new HashSet<Tile>());

        playerActionManager.SelectActionType(PlayerActionType.Undefined);
    }
}
