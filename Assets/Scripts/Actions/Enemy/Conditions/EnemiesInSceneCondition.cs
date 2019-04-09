using System.Linq;
using Sirenix.OdinInspector;


public class EnemiesInSceneCondition : IEnemyActionCondition, IInteractableCondition
{
    public EnemiesInSceneConditionType type = EnemiesInSceneConditionType.NoEnemiesLeft;

    [ShowIf("type", EnemiesInSceneConditionType.NoEnemiesLeftOfType)]
    public string enemyType;

    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
    }

    public void Init(Interactable interactable)
    {
        this.globalCtrl = interactable.sceneCtrl.globalCtrl;
    }


    public bool IsValid()
    {
        SceneState sceneState = this.globalCtrl.sceneCtrl.sceneState;

        switch (this.type)
        {
            case EnemiesInSceneConditionType.NoEnemiesLeft:
                return !sceneState.enemies.Any();
            case EnemiesInSceneConditionType.NoEnemiesLeftOfType:
                return !sceneState.enemies.Where(kvp => (kvp.Value.type == this.enemyType)).Any();
            default:
                return false;
        }
    }
}

public enum EnemiesInSceneConditionType
{
    NoEnemiesLeft,
    NoEnemiesLeftOfType,
}
