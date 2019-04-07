

public class GameOverCondition : ITurnActionCondition
{
    public GameOverConditionType type = GameOverConditionType.Undefined;
    
    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
    }

    public bool IsValid()
    {
        Player player = this.globalCtrl.sceneCtrl.player;

        switch (this.type)
        {
            case GameOverConditionType.isOver:
                return player.visibleRange < 1.5F;
            case GameOverConditionType.isNotOver:
                return player.visibleRange >= 1.5F;
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
