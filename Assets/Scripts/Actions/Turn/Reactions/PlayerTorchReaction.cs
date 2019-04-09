using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerTorchReaction : ITurnActionReaction
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

            yield return player.KillEnemiesOnTiles(playerActionManager.selectedTiles);
        }
    
        playerActionManager.UpdateSelectedTiles(new HashSet<Tile>());

        playerActionManager.SelectActionType(PlayerActionType.Undefined);
    }


    private IEnumerator SlashEffect(Player player, Point targetPoint)
    {
        Vector3 sceneCtrlPosition = this.globalCtrl.sceneCtrl.transform.position;
        // TODO: Validate that it works correctly (if not try +0.5F for x and z)
        player.characterTransform.LookAt(targetPoint.CalcWorldCoord(
            new Vector3(sceneCtrlPosition.x, 0.1F, sceneCtrlPosition.z)
        ));

        Quaternion effectRotation = Quaternion.Euler(
            x: this.effectPrefab.transform.rotation.eulerAngles.x,
            y: player.characterTransform.rotation.eulerAngles.y - 135,
            z: this.effectPrefab.transform.rotation.eulerAngles.z
        );

        GameObject.Instantiate(
            this.effectPrefab,
            // TODO: Validate that it works correctly (if not try +0.5F for x and z)
            player.tile.point.CalcWorldCoord(new Vector3(sceneCtrlPosition.x, 1F, sceneCtrlPosition.z)),
            effectRotation
        );

        yield return new WaitForSeconds(this.effectTimeout);
    }
}
