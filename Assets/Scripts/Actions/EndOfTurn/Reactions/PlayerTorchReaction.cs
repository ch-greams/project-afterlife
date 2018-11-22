using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerTorchReaction : IEndOfTurnReaction
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
            yield return this.SlashEffect(player, playerActionManager.selectedTile.point);

            player.KillEnemiesOnTiles(playerActionManager.selectedTiles);
        }
    
        playerActionManager.UpdateSelectedTiles(new HashSet<Tile>());

        playerActionManager.SelectActionType(PlayerActionType.Undefined);
    }


    private IEnumerator SlashEffect(Player player, Point targetPoint)
    {
        player.characterTransform.LookAt(targetPoint.CalcWorldCoord(0.1F));

        Quaternion effectRotation = Quaternion.Euler(
            x: this.effectPrefab.transform.rotation.eulerAngles.x,
            y: player.characterTransform.rotation.eulerAngles.y - 135,
            z: this.effectPrefab.transform.rotation.eulerAngles.z
        );

        GameObject.Instantiate(this.effectPrefab, player.tile.point.CalcWorldCoord(1F), effectRotation);

        yield return new WaitForSeconds(this.effectTimeout);
    }
}
