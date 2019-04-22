using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerGranadeAction : ITurnActionReaction
{
    public GameObject effectPrefab;
    public float effectTimeout = 0;
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
            yield return this.GranadeEffect(player, playerActionManager.selectedTile.point);

            yield return player.KillEnemiesOnTiles(playerActionManager.selectedTiles);
        }    

        playerActionManager.UpdateSelectedTiles(new HashSet<Tile>());
        playerActionManager.SelectActionType(PlayerActionType.Undefined);
    }

    private IEnumerator GranadeEffect(Player player, Point point)
    {
        Vector3 sceneCtrlPosition = this.globalCtrl.sceneCtrl.transform.position;
        Vector3 targetPosition = point.CalcWorldCoord(sceneCtrlPosition, 0.1F, 0.5F);

        player.characterTransform.LookAt(targetPosition);
        GameObject.Instantiate(this.effectPrefab, targetPosition, this.effectPrefab.transform.rotation);

        yield return new WaitForSeconds(this.effectTimeout);
    }
}
