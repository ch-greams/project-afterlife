

public class GameOverCondition : IEndOfTurnCondition
{
    public GameOverConditionType type = GameOverConditionType.Undefined;
    
    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
    }

    public bool IsValid()
    {
        switch (this.type)
        {
            case GameOverConditionType.isOver:
                return this.globalCtrl.isGameOver;
            case GameOverConditionType.isNotOver:
                return !this.globalCtrl.isGameOver;
            case GameOverConditionType.Undefined:
            default:
                return false;
        }
    }
}

public enum GameOverConditionType
{
    Undefined,
    isOver,
    isNotOver,
}
