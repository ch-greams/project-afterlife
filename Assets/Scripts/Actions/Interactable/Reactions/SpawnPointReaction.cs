using System.Collections;
using UnityEngine;


public class SpawnPointReaction : IInteractableReaction
{
    public SpawnPointReactionType type;
    public GameObject spawnPoint;

    private SceneController sceneCtrl;


    public void Init(Interactable interactable)
    {
        this.sceneCtrl = interactable.sceneCtrl;
    }

    public IEnumerator React()
    {
        switch (this.type)
        {
            case SpawnPointReactionType.EnableSpawnPoint:
                this.spawnPoint.GetComponent<EnemySpawnPoint>().ToggleSpawnPoint(true);
                break;
            case SpawnPointReactionType.DisableSpawnPoint:
                this.spawnPoint.GetComponent<EnemySpawnPoint>().ToggleSpawnPoint(false);
                break;
            default:
                yield return null;
                break;
        }
    }
}

public enum SpawnPointReactionType
{
    EnableSpawnPoint,
    DisableSpawnPoint,
}
