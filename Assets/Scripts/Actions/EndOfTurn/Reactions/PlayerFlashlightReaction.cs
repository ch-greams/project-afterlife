using System.Collections;
using UnityEngine;


public class PlayerFlashlightReaction : IEndOfTurnReaction
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
            player.characterTransform.LookAt(playerActionManager.selectedTile.obj.transform.position);
            player.flashlightRay.SetActive(true);

            player.UseFlashlight(playerActionManager.selectedTile.point);

            yield return new WaitForSeconds(this.animationTimeout);

            player.flashlightRay.SetActive(false);
        }
    
        playerActionManager.SelectActionType(PlayerActionType.Undefined);
    }
}
